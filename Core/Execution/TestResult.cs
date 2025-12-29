// <copyright file="TestResult.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace Core.Execution
{
    /// <summary>
    /// Contains execution information about a single test.
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Full name of the test (ClassName.MethodName).
        /// </summary>
        public string TestName { get; }

        /// <summary>
        /// Final status of the test.
        /// </summary>
        public TestStatus Status { get; }

        /// <summary>
        /// Execution time in milliseconds.
        /// </summary>
        public long ExecutionTimeMs { get; }

        /// <summary>
        /// Optional message (failure reason or ignore reason).
        /// </summary>
        public string? Message { get; }

        public TestResult(
            string testName,
            TestStatus status,
            long executionTimeMs,
            string? message = null)
        {
            TestName = testName;
            Status = status;
            ExecutionTimeMs = executionTimeMs;
            Message = message;
        }
    }
}