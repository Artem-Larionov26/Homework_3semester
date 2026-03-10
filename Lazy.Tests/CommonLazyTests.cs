// <copyright file="CommonLazyTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using NUnit.Framework;

namespace LazyImplementation.Tests;

public abstract class CommonLazyTests
{
    protected abstract ILazy<T> CreateLazy<T>(Func<T> supplier);

    [Test]
    public void SupplierCalledOnlyOnce()
    {
        int callCount = 0;

        Func<int> supplier = () =>
        {
            callCount++;
            return 42;
        };

        var lazy = CreateLazy(supplier);

        var first = lazy.Get();
        var second = lazy.Get();
        var third = lazy.Get();

        Assert.That(first, Is.EqualTo(42));
        Assert.That(second, Is.EqualTo(42));
        Assert.That(third, Is.EqualTo(42));

        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public void SupportsNullValue()
    {
        int callCount = 0;

        Func<string?> supplier = () =>
        {
            callCount++;
            return null;
        };

        var lazy = CreateLazy(supplier);

        var first = lazy.Get();
        var second = lazy.Get();

        Assert.That(first, Is.Null);
        Assert.That(second, Is.Null);

        Assert.That(callCount, Is.EqualTo(1));
    }

    [Test]
    public void ReturnsSameInstance()
    {
        var obj = new object();

        Func<object> supplier = () => obj;

        var lazy = CreateLazy(supplier);

        var first = lazy.Get();
        var second = lazy.Get();

        Assert.That(ReferenceEquals(first, second), Is.True);
    }
}