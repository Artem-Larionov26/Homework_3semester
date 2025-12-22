// <copyright file="DirectoryEntry.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace SimpleFTP
{
    public record DirectoryEntry(string Name, bool IsDirectory);
}