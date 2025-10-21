// <copyright file="MyThreadPool.cs" company="Kalinin Andrew">
// Copyright (c) Kalinin Andrew. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// A simple task pool with a fixed number of threads.
/// </summary>
public class MyThreadPool : IDisposable
{
    private readonly Thread[] workers;

    private readonly Queue<Action> taskQueue = new Queue<Action>();

    private readonly object locker = new object();

    private volatile bool isShutdownInitiated = false;

    private bool AcceptingTasks => !isShutdownInitiated;

    /// <summary>
    /// Creates and starts n worker threads.
    /// </summary>
    /// <param name="workerCount">The number of work streams should be >= 1</param>
    public MyThreadPool(int workerCount)
    {
        if (workerCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(workerCount));
        }
        workers = new Thread[workerCount];

        for (int i = 0; i < workerCount; i++)
        {
            workers[i] = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = $"MyThreadPool-Worker-{i}"
            };
            workers[i].Start();
        }
    }

    /// <summary>
    /// Sends a function for execution, returns an object IMyTask<TResult>.
    /// If Shutdown has already been initiated, it throws InvalidOperationException (we do not accept new tasks).
    /// </summary>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        lock (locker)
        {
            if (!isShutdownInitiated)
            {
                var task = new MyTask<TResult>(this, function);
                EnqueueInternal(task.RunAsRoot);
                return task;
            }
            else
            {
                throw new InvalidOperationException("ThreadPool is shutting down — cannot accept new tasks.");
            }
        }
    }

    /// <summary>
    /// An internal method for placing an Action in a shared queue and waking up waiting threads.
    /// Thread-safe (called from outside the locker or inside).
    /// </summary>
    private void EnqueueInternal(Action work)
    {
        lock (locker)
        {
            taskQueue.Enqueue(work);
            Monitor.Pulse(locker);
        }
    }

    /// <summary>
    /// The work cycle of each thread: retrieves tasks from the queue and executes them.
    /// Ends when Shutdown is initiated and the queue is empty.
    /// </summary>
    private void WorkerLoop()
    {
        while (true)
        {
            Action work = null;
            lock (locker)
            {
                while (taskQueue.Count == 0 && !isShutdownInitiated)
                {
                    Monitor.Wait(locker);
                }

                if (taskQueue.Count == 0 && isShutdownInitiated)
                {
                    return;
                }

                if (taskQueue.Count > 0)
                    work = taskQueue.Dequeue();
            }

            try
            {
                work?.Invoke();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Pool shutdown — collaborative shutdown.
    /// - We forbid accepting new Submissions.
    /// - We wait until the queue is empty and all worker threads finish executing current tasks.
    /// Shutdown blocks the calling thread until it is fully completed.
    /// </summary>
    public void Shutdown()
    {
        lock (locker)
        {
            isShutdownInitiated = true;
            Monitor.PulseAll(locker);
        }

        foreach (var t in workers)
        {
            if (t == null)
            {
                continue;
            }
            try
            {
                t.Join();
            }
            catch (ThreadStateException)
            {
            }
        }
    }

    /// <summary>
    /// Dispose calls Shutdown for ease of use in using.
    /// </summary>
    public void Dispose()
    {
        Shutdown();
    }

    #region Вложенная реализация IMyTask<TResult> и MyTask<TResult>

    /// <summary>
    /// The task interface that MyTask should implement.
    /// Here it is defined in a file for self—sufficiency - you can put it in IMyTask.cs.
    /// </summary>
    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
    }

    /// <summary>
    /// Implementation of IMyTask<TResult>.
    /// </summary>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly MyThreadPool _pool;

        private readonly Func<TResult> _function;

        private volatile bool isCompleted = false;

        private TResult result;

        private Exception exception;

        private readonly ManualResetEventSlim completedEvent = new ManualResetEventSlim(false);

        private readonly List<Action<TResult>> pendingContinuations = new List<Action<TResult>>();

        private readonly object _sync = new object();

        /// <summary>
        /// Creates a task linked to the pool. 
        /// </summary>
        public MyTask(MyThreadPool pool, Func<TResult> function)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _function = function;
        }

        public bool IsCompleted => isCompleted;

        /// <summary>
        /// Blocks the calling thread if the task has not completed yet.
        /// If the calculation is completed with an exception, it throws an AggregateException.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (!isCompleted)
                {
                    completedEvent.Wait();
                }

                if (exception != null)
                {
                    throw new AggregateException(exception);
                }

                return result;
            }
        }

        /// <summary>
        /// Submit calls this method — it runs the root function in the workflow.
        /// </summary>
        public void RunAsRoot()
        {
            try
            {
                TResult r = _function();
                CompleteSuccessfully(r);
            }
            catch (Exception ex)
            {
                CompleteExceptionally(ex);
            }
        }

        /// <summary>
        /// Completes the task successfully and initiates the execution/delivery of all registered continuations.
        /// </summary>
        private void CompleteSuccessfully(TResult r)
        {
            List<Action<TResult>> continuationsCopy = null;

            lock (_sync)
            {
                if (isCompleted)
                {
                    return;
                }

                result = r;
                isCompleted = true;
                completedEvent.Set();

                if (pendingContinuations.Count > 0)
                {
                    continuationsCopy = new List<Action<TResult>>(pendingContinuations);
                    pendingContinuations.Clear();
                }
            }

            if (continuationsCopy != null)
            {
                foreach (var cont in continuationsCopy)
                {
                    try
                    {
                        cont?.Invoke(r);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Completes the task with an exception and tries to notify continuations in the same way.,
        /// which, when encountering a shutdown/error, will receive an error.
        /// </summary>
        private void CompleteExceptionally(Exception ex)
        {
            List<Action<TResult>> continuationsCopy = null;

            lock (_sync)
            {
                if (isCompleted)
                {
                    return;
                }
                exception = ex;
                isCompleted = true;
                completedEvent.Set();

                if (pendingContinuations.Count > 0)
                {
                    continuationsCopy = new List<Action<TResult>>(pendingContinuations);
                    pendingContinuations.Clear();
                }
            }

            if (continuationsCopy != null)
            {
                foreach (var cont in continuationsCopy)
                {
                    try
                    {
                        cont?.Invoke(default(TResult));
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Registration of ContinueWith. It does not block.
        /// Returns a new task (which will be completed in the pool).
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            var continuationTask = new MyTask<TNewResult>(_pool, null);

            Action<TResult> registration = (parentResult) =>
            {
                lock (_pool.locker)
                {
                    if (_pool.AcceptingTasks)
                    {
                        _pool.EnqueueInternal(() =>
                        {
                            try
                            {
                                TResult val;
                                Exception parentEx = null;
                                try
                                {
                                }
                                catch
                                {
                                }

                                TNewResult contRes = continuation(parentResult);
                                continuationTask.SetResult(contRes);
                            }
                            catch (Exception ex)
                            {
                                continuationTask.SetException(ex);
                            }
                        });
                    }
                    else
                    {
                        continuationTask.SetException(new InvalidOperationException("ThreadPool is shutting down — continuation will not be executed."));
                    }
                }
            };

            bool callImmediately = false;
            TResult parentResultSnapshot = default(TResult);

            lock (_sync)
            {
                if (isCompleted)
                {
                    callImmediately = true;
                    parentResultSnapshot = result;
                }
                else
                {
                    pendingContinuations.Add(registration);
                }
            }

            if (callImmediately)
            {
                registration(parentResultSnapshot);
            }

            return continuationTask;
        }

        /// <summary>
        /// Sets the successful result "manually" (using the continuation runner).
        /// </summary>
        private void SetResult(TResult r)
        {
            CompleteSuccessfully(r);
        }

        /// <summary>
        /// Sets the exception "manually".
        /// </summary>
        private void SetException(Exception ex)
        {
            CompleteExceptionally(ex);
        }
    }
    #endregion
}