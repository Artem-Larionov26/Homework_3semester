// <copyright file="TestMethodInfo.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Reflection;
using Attributes;

namespace Core.Models
{
    /// <summary>
    /// Represents a single test method discovered via reflection.
    /// Stores both reflection metadata and the associated Test attribute.
    /// </summary>
    public class TestMethodInfo
    {
        /// <summary>
        /// Reflection information about the test method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Test attribute applied to the method.
        /// </summary>
        public TestAttribute Attribute { get; }

        public TestMethodInfo(MethodInfo method, TestAttribute attribute)
        {
            Method = method;
            Attribute = attribute;
        }
    }
}