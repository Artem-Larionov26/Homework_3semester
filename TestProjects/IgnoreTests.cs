// <copyright file="IgnoreTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using Attributes;

namespace TestProjects
{
    /// <summary>
    /// Tests ignored test handling.
    /// </summary>
    public class IgnoreTests
    {
        [Test(Ignore = "Feature under development")]
        public void IgnoredTestIsNotExecuted()
        {
            throw new Exception("This test must not be executed");
        }
    }
}