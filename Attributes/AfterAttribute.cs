// <copyright file="AfterAttribute.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace Attributes
{
    /// <summary>
    /// Marks a method that should be executed
    /// after each test method in the same class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class AfterAttribute : Attribute
    {
    }
}