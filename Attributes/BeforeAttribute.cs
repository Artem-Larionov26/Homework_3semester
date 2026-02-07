// <copyright file="BeforeAttribute.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Attributes;

using System;

/// <summary>
/// Marks a method that should be executed
/// before each test method in the same class.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class BeforeAttribute : Attribute
{
}