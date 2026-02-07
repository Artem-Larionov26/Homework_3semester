// <copyright file="ExceptionTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace TestProjects;

using System;
using Attributes;

/// <summary>
/// Tests expected exception handling.
/// </summary>
public class ExceptionTests
{
    [TestAttribute(Expected = typeof(InvalidOperationException))]
    public void ExpectedExceptionIsHandledCorrectly()
    {
        throw new InvalidOperationException();
    }

    [TestAttribute(Expected = typeof(InvalidOperationException))]
    public void UnexpectedExceptionFailsTest()
    {
        throw new ArgumentException();
    }

    [TestAttribute]
    public void MissingExpectedExceptionFailsTest()
    {
    }
}