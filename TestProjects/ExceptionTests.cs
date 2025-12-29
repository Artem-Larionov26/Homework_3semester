// <copyright file="ExceptionTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using Attributes;

namespace TestProjects
{
    /// <summary>
    /// Tests expected exception handling.
    /// </summary>
    public class ExceptionTests
    {
        [Test(Expected = typeof(InvalidOperationException))]
        public void ExpectedExceptionIsHandledCorrectly()
        {
            throw new InvalidOperationException();
        }

        [Test(Expected = typeof(InvalidOperationException))]
        public void UnexpectedExceptionFailsTest()
        {
            throw new ArgumentException();
        }

        [Test]
        public void MissingExpectedExceptionFailsTest()
        {
        }
    }
}