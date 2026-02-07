// <copyright file="IgnoreTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace TestProjects;

using System;
using Attributes;

/// <summary>
/// Tests ignored test handling.
/// </summary>
public class IgnoreTests
{
    [TestAttribute(Ignore = "Feature under development")]
    public void IgnoredTestIsNotExecuted()
    {
        throw new Exception("This test must not be executed");
    }
}