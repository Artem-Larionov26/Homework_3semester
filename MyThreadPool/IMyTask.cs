// <copyright file="IMyTask.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace MyThreadPool;

/// <summary>
/// Represents an asynchronous computation executed by <see cref="MyThreadPool"/>.
/// Provides access to the computation result and allows registering continuations.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the task.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether the task has completed execution.
    /// Returns <c>true</c> if the task has finished successfully or with an exception.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task.
    /// If the task has not completed yet, blocks the calling thread until completion.
    /// If the task completed with an exception, throws an <see cref="AggregateException"/>
    /// containing the original exception.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// Creates a continuation task that will be executed after the current task completes successfully.
    /// The continuation is scheduled for execution in the same thread pool and does not block the calling thread.
    /// </summary>
    /// <typeparam name="TNewResult">The type of the result produced by the continuation.</typeparam>
    /// <param name="continuation">
    /// A function that is applied to the result of the current task.
    /// </param>
    /// <returns>
    /// A new task representing the continuation computation.
    /// </returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}