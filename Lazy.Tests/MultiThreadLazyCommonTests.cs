// <copyright file="MultiThreadLazyCommonTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace LazyImplementation.Tests;

public class MultiThreadLazyCommonTests : CommonLazyTests
{
    protected override ILazy<T> CreateLazy<T>(Func<T> supplier)
    {
        return new MultiThreadLazy<T>(supplier);
    }
}