// <copyright file="TestMethodInfo.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Models;

using System.Reflection;
using Attributes;

/// <summary>
/// Represents a single test method discovered via reflection.
/// Stores both reflection metadata and the associated Test attribute.
/// </summary>
public record TestMethodInfo(MethodInfo Method, TestAttribute Attribute);