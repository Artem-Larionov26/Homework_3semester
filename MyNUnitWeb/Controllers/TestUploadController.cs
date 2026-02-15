// <copyright file="TestUploadController.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Controllers;

using Microsoft.AspNetCore.Mvc;
using MyNUnitWeb.Services;

/// <summary>
/// Handles uploading and running test assemblies.
/// </summary>
[ApiController]
[Route("api/tests")]
public sealed class TestUploadController : ControllerBase
{
    private readonly ITestRunService service;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestUploadController"/> class.
    /// </summary>
    /// <param name="service">Service responsible for test execution.</param>
    public TestUploadController(ITestRunService service)
        => this.service = service;

    /// <summary>
    /// Returns the history of all test runs.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing a list of test run results.
    /// </returns>
    [HttpGet("history")]
    public IActionResult GetHistory()
        => this.Ok(this.service.GetHistory());

    /// <summary>
    /// Uploads a test assembly for further execution.
    /// </summary>
    /// <param name="file">DLL file containing tests.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating whether the upload operation was successful.
    /// </returns>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            await this.service.UploadAsync(file);
            return this.Ok();
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return this.Problem(ex.Message);
        }
    }

    /// <summary>
    /// Runs all previously uploaded test assemblies.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the results of the test execution.
    /// </returns>
    [HttpPost("run")]
    public IActionResult Run()
    {
        try
        {
            return this.Ok(this.service.RunTests());
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return this.Problem(ex.Message);
        }
    }
}