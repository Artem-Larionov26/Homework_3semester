// <copyright file="ITestRunService.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using Microsoft.AspNetCore.Http;
using MyNUnitWeb.Models;

/// <summary>
/// Provides functionality for uploading test assemblies,
/// running tests, and retrieving test run history.
/// </summary>
public interface ITestRunService
{
    /// <summary>
    /// Uploads a test assembly to the server.
    /// </summary>
    /// <param name="file">Assembly file containing tests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UploadAsync(IFormFile file);

    /// <summary>
    /// Runs all uploaded test assemblies.
    /// </summary>
    /// <returns>Result of the test run.</returns>
    TestRunDto RunTests();

    /// <summary>
    /// Gets the history of all test runs.
    /// </summary>
    /// <returns>Read-only list of test run results.</returns>
    IReadOnlyList<TestRunDto> GetHistory();
}