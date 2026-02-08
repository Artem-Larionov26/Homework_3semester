// <copyright file="TestStatus.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Execution;

/// <summary>
/// Represents the final execution status of a test.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// Test finished successfully without errors.
    /// </summary>
    Passed,

    /// <summary>
    /// Test finished, but its assertions failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Test execution resulted in an unexpected error.
    /// </summary>
    Errored,

    /// <summary>
    /// Test was skipped and not executed.
    /// </summary>
    Ignored,
}