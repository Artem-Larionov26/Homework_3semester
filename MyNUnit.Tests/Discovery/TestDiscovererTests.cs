// <copyright file="TestDiscovererTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnit.Tests.Discovery;

using System.IO;
using System.Linq;
using Core.Discovery;
using NUnit.Framework;

/// <summary>
/// Tests for test discovery via reflection.
/// </summary>
public class TestDiscovererTests
{
    [Test]
    public void DiscoverFindsTestClasses()
    {
        var discoverer = new TestDiscoverer();
        var path = Path.GetDirectoryName(typeof(TestDiscovererTests).Assembly.Location)!;

        var result = discoverer.Discover(path);

        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Any(tc => tc.Tests.Any()), Is.True);
    }
}