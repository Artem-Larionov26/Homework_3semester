// <copyright file="DirectoryEntry.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace SimpleFTP;

/// <summary>
/// Represents a file system entry (file or directory).
/// </summary>
/// <param name="Name">Name of the file or directory.</param>
/// <param name="IsDirectory">True if the entry is a directory.</param>
public record DirectoryEntry(string Name, bool IsDirectory);