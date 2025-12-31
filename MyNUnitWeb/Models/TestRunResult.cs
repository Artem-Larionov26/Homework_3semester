// <copyright file="TestRunResult.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using Core.Execution;
using Core.Models;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Represents the result of a single test run.
    /// </summary>
    public class TestRunResult
    {
        /// <summary>
        /// Date and time when the test run was started.
        /// </summary>
        public DateTime StartedAt { get; init; }

        /// <summary>
        /// Results of all executed tests.
        /// </summary>
        public IReadOnlyList<TestResult> Results { get; init; } = [];
    }
}