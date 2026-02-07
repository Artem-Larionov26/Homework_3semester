// <copyright file="TestStatus.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Execution;

/// <summary>
/// Represents the final execution status of a test.
/// </summary>
public enum TestStatus
{
    Passed,
    Failed,
    Errored,
    Ignored
}