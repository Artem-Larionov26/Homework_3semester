// <copyright file="TestResult.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Execution;

using System;

/// <summary>
/// Contains execution information about a single test.
/// </summary>
public record TestResult(
    string TestName,
    TestStatus Status,
    long ExecutionTimeMs,
    string? Message = null);