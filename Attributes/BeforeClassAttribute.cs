// <copyright file="BeforeClassAttribute.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Attributes;

using System;

/// <summary>
/// Marks a method that should be executed once
/// before any tests in the class are run.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class BeforeClassAttribute : Attribute
{
}