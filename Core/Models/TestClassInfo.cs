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
    public List<TestMethodInfo> Tests { get; } = new();

    public List<MethodInfo> BeforeMethods { get; } = new();

    public List<MethodInfo> AfterMethods { get; } = new();

    public List<MethodInfo> BeforeClassMethods { get; } = new();

    public List<MethodInfo> AfterClassMethods { get; } = new();

}