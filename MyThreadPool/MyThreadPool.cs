// <copyright file="MyThreadPool.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool;

/// <summary>
/// A simple fixed-size thread pool for executing tasks.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly Thread[] workers;
    private readonly Queue<Action> taskQueue = new();
    private readonly object locker = new();

    private volatile bool isShutdownInitiated;

    /// <summary>
    /// Initializes the thread pool and starts worker threads.
    /// </summary>
    /// <param name="workerCount">Number of worker threads.</param>
    public MyThreadPool(int workerCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(workerCount);

        workers = new Thread[workerCount];

        for (var i = 0; i < workerCount; i++)
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
    /// Submits a task for execution.
    /// </summary>
    /// <typeparam name="TResult">Task result type.</typeparam>
    /// <param name="function">Function to execute.</param>
    /// <returns>An object representing the submitted task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        lock (locker)
        {
            if (isShutdownInitiated)
            {
                throw new InvalidOperationException("ThreadPool is shutting down.");
            }

            var task = new MyTask<TResult>(this, function);
            Enqueue(task.Execute);
            return task;
        }
    }

    private void Enqueue(Action action)
    {
        lock (locker)
        {
            taskQueue.Enqueue(action);
            Monitor.Pulse(locker);
        }
    }

    private void WorkerLoop()
    {
        while (true)
        {
            Action work;

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

                work = taskQueue.Dequeue();
            }

            try
            {
                work();
            }
            catch
            {
                // Exceptions are handled inside tasks
            }
        }
    }

    /// <summary>
    /// Initiates cooperative shutdown and waits for all workers to stop.
    /// </summary>
    public void Shutdown()
    {
        lock (locker)
        {
            isShutdownInitiated = true;
            Monitor.PulseAll(locker);
        }

        foreach (var worker in workers)
        {
            worker.Join();
        }
    }

    public void Dispose() => Shutdown();

    private sealed class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly MyThreadPool pool;
        private Func<TResult>? function;

        private TResult? result;
        private Exception? exception;
        private volatile bool isCompleted;

        private readonly ManualResetEventSlim completionEvent = new(false);
        private readonly List<Action> continuations = new();
        private readonly Lock sync = new();

        public MyTask(MyThreadPool pool, Func<TResult> function)
        {
            this.pool = pool;
            this.function = function;
        }

        public bool IsCompleted => isCompleted;

        public TResult Result
        {
            get
            {
                completionEvent.Wait();

                if (exception != null)
                {
                    throw new AggregateException(exception);
                }

                return result!;
            }
        }

        public void Execute()
        {
            try
            {
                var r = function!();
                CompleteSuccessfully(r);
            }
            catch (Exception ex)
            {
                CompleteExceptionally(ex);
            }
            finally
            {
                function = null;
            }
        }

        private void CompleteSuccessfully(TResult value)
        {
            List<Action> toRun;

            lock (sync)
            {
                result = value;
                isCompleted = true;
                toRun = new List<Action>(continuations);
                continuations.Clear();
            }

            completionEvent.Set();

            foreach (var cont in toRun)
            {
                pool.Enqueue(cont);
            }
        }

        private void CompleteExceptionally(Exception ex)
        {
            lock (sync)
            {
                exception = ex;
                isCompleted = true;
                continuations.Clear();
            }

            completionEvent.Set();
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            ArgumentNullException.ThrowIfNull(continuation);

            var nextTask = new MyTask<TNewResult>(pool, () =>
            {
                if (exception != null)
                {
                    throw exception;
                }

                return continuation(Result);
            });

            Action schedule = () =>
            {
                lock (pool.locker)
                {
                    if (pool.isShutdownInitiated)
                    {
                        nextTask.CompleteExceptionally(
                            new InvalidOperationException("ThreadPool is shutting down."));
                    }
                    else
                    {
                        pool.Enqueue(nextTask.Execute);
                    }
                }
            };

            lock (sync)
            {
                if (isCompleted)
                {
                    schedule();
                }
                else
                {
                    continuations.Add(schedule);
                }
            }

            return nextTask;
        }
    }
}