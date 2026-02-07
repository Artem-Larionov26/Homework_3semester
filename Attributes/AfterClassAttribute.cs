// <copyright file="AfterClassAttribute.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Attributes;

using System;

/// <summary>
/// Marks a method that should be executed once
/// after all tests in the class have finished.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class AfterClassAttribute : Attribute
{
}