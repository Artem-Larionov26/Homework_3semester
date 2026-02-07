// <copyright file="TestAttribute.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Attributes;

using System;

/// <summary>
/// Marks a method as a test.
/// This attribute is discovered via reflection at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets expected exception type.
    /// If the test throws this exception (or a derived one),
    /// the test is considered passed.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets if not null, the test will be skipped.
    /// The string value is used as the reason for ignoring the test.
    /// </summary>
    public string? Ignore { get; set; }
}