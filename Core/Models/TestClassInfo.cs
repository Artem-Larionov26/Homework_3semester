// <copyright file="TestClassInfo.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Models
{
    /// <summary>
    /// Represents a test class containing test methods
    /// and lifecycle methods (Before, After, etc.).
    /// </summary>
    public class TestClassInfo
    {
        /// <summary>
        /// CLR type of the test class.
        /// </summary>
        public Type ClassType { get; }

        /// <summary>
        /// All discovered test methods in the class.
        /// </summary>
        public List<TestMethodInfo> Tests { get; } = new();

        /// <summary>
        /// Methods executed before each test.
        /// </summary>
        public List<MethodInfo> BeforeMethods { get; } = new();

        /// <summary>
        /// Methods executed after each test.
        /// </summary>
        public List<MethodInfo> AfterMethods { get; } = new();

        /// <summary>
        /// Static methods executed once before all tests in the class.
        /// </summary>
        public List<MethodInfo> BeforeClassMethods { get; } = new();

        /// <summary>
        /// Static methods executed once after all tests in the class.
        /// </summary>
        public List<MethodInfo> AfterClassMethods { get; } = new();

        public TestClassInfo(Type classType)
        {
            ClassType = classType;
        }
    }
}