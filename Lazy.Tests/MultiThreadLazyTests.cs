// <copyright file="MultiThreadLazyTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LazyImplementation.Tests;

[TestFixture]
public class MultiThreadLazyTests
{
    [Test]
    public void ShouldBeThreadSafe()
    {
        int callCount = 0;
        var barrier = new Barrier(10);

        Func<int> supplier = () =>
        {
            Interlocked.Increment(ref callCount);
            Thread.Sleep(50);
            return 777;
        };

        var lazy = new MultiThreadLazy<int>(supplier);
        int[] results = new int[10];

        Parallel.For(0, 10, i =>
        {
            barrier.SignalAndWait();
            results[i] = lazy.Get();
        });

        foreach (var result in results)
            Assert.That(result, Is.EqualTo(777));

        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public void ThrowsIfSupplierIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var lazy = new MultiThreadLazy<int>(null!);
        });
    }
}