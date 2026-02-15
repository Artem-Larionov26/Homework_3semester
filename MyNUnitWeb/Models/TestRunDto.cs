// <copyright file="TestRunDto.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace MyNUnitWeb.Models;

/// <summary>
/// Represents a single test run.
/// </summary>
public sealed record TestRunDto(
    DateTime StartedAt,
    IReadOnlyList<TestResultDto> Results);