// <copyright file="TestResultDto.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Models;

using Core.Execution;

/// <summary>
/// Represents a data transfer object for a single test result.
/// </summary>
public sealed class TestResultDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResultDto"/> class.
    /// </summary>
    /// <param name="testName">Name of the test.</param>
    /// <param name="status">Execution status of the test.</param>
    /// <param name="message">Additional message or error description.</param>
    public TestResultDto(string testName, TestStatus status, string? message)
    {
        this.TestName = testName;
        this.Status = status;
        this.Message = message;
    }

    /// <summary>
    /// Gets the test name.
    /// </summary>
    public string TestName { get; }

    /// <summary>
    /// Gets the execution status of the test.
    /// </summary>
    public TestStatus Status { get; }

    /// <summary>
    /// Gets the message associated with the test result.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the execution duration in milliseconds.
    /// </summary>
    public long Duration { get; }

    /// <summary>
    /// Creates a <see cref="TestResultDto"/> from a Core test result.
    /// </summary>
    /// <param name="result">Core test result.</param>
    /// <returns>Converted test result DTO.</returns>
    public static TestResultDto FromCore(TestResult result)
        => new(result.TestName, result.Status, result.Message);
}