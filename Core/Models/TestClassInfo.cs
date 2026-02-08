// <copyright file="TestClassInfo.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Models;

using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Represents a test class containing test methods
/// and lifecycle methods (Before, After, etc.).
/// </summary>
public record TestClassInfo(Type ClassType)
{
    /// <summary>
    /// Gets the list of test methods defined in the test class.
    /// </summary>
    public List<TestMethodInfo> Tests { get; } = new();

    /// <summary>
    /// Gets the list of methods executed before each test.
    /// </summary>
    public List<MethodInfo> BeforeMethods { get; } = new();

    /// <summary>
    /// Gets the list of methods executed after each test.
    /// </summary>
    public List<MethodInfo> AfterMethods { get; } = new();

    /// <summary>
    /// Gets the list of methods executed once before all tests in the class.
    /// </summary>
    public List<MethodInfo> BeforeClassMethods { get; } = new();

    /// <summary>
    /// Gets the list of methods executed once after all tests in the class.
    /// </summary>
    public List<MethodInfo> AfterClassMethods { get; } = new();
}