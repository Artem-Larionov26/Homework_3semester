// <copyright file="TestUploadController.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Core.Discovery;
using Core.Execution;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Controllers
{
    /// <summary>
    /// Handles uploading of test assemblies to the server.
    /// </summary>
    [ApiController]
    [Route("api/tests")]
    public class TestUploadController : ControllerBase
    {
        private static readonly List<TestRunResult> TestRunsHistory = new();

        /// <summary>
        /// Returns history of all test runs.
        /// </summary>
        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            return Ok(TestRunsHistory);
        }

        private static readonly string UploadDirectory =
            Path.Combine(AppContext.BaseDirectory, "UploadedAssemblies");

        /// <summary>
        /// Uploads a test assembly (.dll) to the server.
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            if (!file.FileName.EndsWith(".dll"))
            {
                return BadRequest("Only .dll files are allowed.");
            }

            Directory.CreateDirectory(UploadDirectory);

            var filePath = Path.Combine(UploadDirectory, file.FileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok($"File '{file.FileName}' uploaded successfully.");
        }

        /// <summary>
        /// Runs all uploaded test assemblies.
        /// </summary>
        [HttpPost("run")]
        public IActionResult RunTests()
        {
            if (!Directory.Exists(UploadDirectory))
            {
                return BadRequest("No assemblies uploaded.");
            }

            var discoverer = new TestDiscoverer();
            var testClasses = discoverer.Discover(UploadDirectory);

            var runner = new TestRunner();
            var results = runner.Run(testClasses);

            var runResult = new TestRunResult
            {
                StartedAt = DateTime.UtcNow,
                Results = results
            };

            TestRunsHistory.Add(runResult);

            return Ok(runResult);
        }
    }
}