// <copyright file="MyThreadPoolTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using MyThreadPool;

namespace MyThreadPool.Tests;

[TestFixture]
public class MyThreadPoolTests
{
    [Test]
    public void Submit_TaskIsExecuted_ReturnsCorrectResult()
    {
        using var pool = new MyThreadPool(2);
        var task = pool.Submit(() => 2 + 2);

        Assert.That(task.Result, Is.EqualTo(4));
        Assert.That(task.IsCompleted, Is.True);
    }

    [Test]
    public void Result_BlocksUntilTaskIsCompleted()
    {
        using var pool = new MyThreadPool(1);
        var task = pool.Submit(() =>
        {
            Thread.Sleep(100);
            return 42;
        });

        Assert.That(task.Result, Is.EqualTo(42));
    }

    [Test]
    public void Exception_IsWrappedIntoAggregateException()
    {
        using var pool = new MyThreadPool(1);
        var task = pool.Submit<int>(() => throw new InvalidOperationException());

        Assert.Throws<AggregateException>(() => _ = task.Result);
    }

    [Test]
    public void ContinueWith_WorksCorrectly()
    {
        using var pool = new MyThreadPool(2);
        var task = pool.Submit(() => 10).ContinueWith(x => x * 2);

        Assert.That(task.Result, Is.EqualTo(20));
    }

    [Test]
    public void MultipleContinuations_WorkCorrectly()
    {
        using var pool = new MyThreadPool(2);
        var baseTask = pool.Submit(() => 5);

        var t1 = baseTask.ContinueWith(x => x + 1);
        var t2 = baseTask.ContinueWith(x => x * 2);

        Assert.That(t1.Result, Is.EqualTo(6));
        Assert.That(t2.Result, Is.EqualTo(10));
    }

    [Test]
    public void Shutdown_ForbidsSubmittingNewTasks()
    {
        var pool = new MyThreadPool(1);
        pool.Shutdown();

        Assert.Throws<InvalidOperationException>(() => pool.Submit(() => 1));
    }

    [Test]
    public void Shutdown_WaitsForRunningTasks()
    {
        var pool = new MyThreadPool(1);
        var task = pool.Submit(() =>
        {
            Thread.Sleep(100);
            return 123;
        });

        pool.Shutdown();
        Assert.That(task.Result, Is.EqualTo(123));
    }

    [Test]
    public void ThreadPool_UsesAtLeastGivenNumberOfThreads()
    {
        const int threadCount = 4;
        using var pool = new MyThreadPool(threadCount);

        var threads = new ConcurrentDictionary<int, bool>();

        var tasks = Enumerable.Range(0, threadCount)
            .Select(_ => pool.Submit(() =>
            {
                threads[Thread.CurrentThread.ManagedThreadId] = true;
                Thread.Sleep(50);
                return 0;
            }))
            .ToArray();

        foreach (var task in tasks)
        {
            _ = task.Result;
        }

        Assert.That(threads.Count, Is.GreaterThanOrEqualTo(threadCount));
    }
}