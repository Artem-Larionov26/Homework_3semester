// <copyright file="SingleThreadLazyTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using NUnit.Framework;

namespace LazyImplementation.Tests;

[TestFixture]
public class SingleThreadLazyTests
{
    [Test]
    public void ThrowsIfSupplierIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var lazy = new SingleThreadLazy<int>(null);
        });
    }
}