// <copyright file="SingleThreadLazyCommonTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace LazyImplementation.Tests;

public class SingleThreadLazyCommonTests : CommonLazyTests
{
    protected override ILazy<T> CreateLazy<T>(Func<T> supplier)
    {
        return new SingleThreadLazy<T>(supplier);
    }
}