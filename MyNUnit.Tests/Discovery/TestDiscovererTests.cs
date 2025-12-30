// <copyright file="TestDiscovererTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.IO;
using System.Linq;
using Core.Discovery;
using NUnit.Framework;

namespace MyNUnit.Tests.Discovery
{
    /// <summary>
    /// Tests for test discovery via reflection.
    /// </summary>
    public class TestDiscovererTests
    {
        [Test]
        public void Discover_FindsTestClasses()
        {
            var discoverer = new TestDiscoverer();
            var path = Path.GetDirectoryName(typeof(TestDiscovererTests).Assembly.Location)!;

            var result = discoverer.Discover(path);

            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Any(tc => tc.Tests.Any()), Is.True);
        }
    }
}