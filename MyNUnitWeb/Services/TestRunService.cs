// <copyright file="TestRunService.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Services;

using Core.Discovery;
using Core.Execution;
using MyNUnitWeb.Models;

/// <summary>
/// Service responsible for managing test assemblies,
/// executing tests, and storing test run history.
/// </summary>
public sealed class TestRunService : ITestRunService
{
    /// <summary>
    /// Directory where uploaded test assemblies are stored.
    /// </summary>
    private static readonly string UploadDirectory =
        Path.Combine(AppContext.BaseDirectory, "UploadedAssemblies");

    /// <summary>
    /// History of all test runs.
    /// </summary>
    private readonly List<TestRunDto> history = new();

    /// <inheritdoc/>
    public async Task UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty.");
        }

        if (!file.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Only .dll files are allowed.");
        }

        Directory.CreateDirectory(UploadDirectory);

        var filePath = Path.Combine(UploadDirectory, file.FileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    /// <inheritdoc/>
    public TestRunDto RunTests()
    {
        if (!Directory.Exists(UploadDirectory))
        {
            throw new InvalidOperationException("No assemblies uploaded.");
        }

        var discoverer = new TestDiscoverer();
        var testClasses = discoverer.Discover(UploadDirectory);

        var runner = new TestRunner();
        var results = runner.Run(testClasses);

        var run = new TestRunDto(
            DateTime.UtcNow,
            results.Select(TestResultDto.FromCore).ToList());

        this.history.Add(run);

        return run;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TestRunDto> GetHistory()
        => this.history;
}