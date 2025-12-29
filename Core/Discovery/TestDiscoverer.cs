// <copyright file="TestDiscoverer.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Attributes;
using Core.Models;

namespace Core.Discovery
{
    /// <summary>
    /// Discovers test classes and test methods in assemblies using reflection.
    /// </summary>
    public class TestDiscoverer
    {
        /// <summary>
        /// Loads all assemblies from the specified path and discovers tests in them.
        /// </summary>
        public IReadOnlyList<TestClassInfo> Discover(string path)
        {
            var result = new List<TestClassInfo>();

            foreach (var assembly in LoadAssemblies(path))
            {
                foreach (var type in assembly.GetTypes())
                {
                    var testClass = DiscoverTestClass(type);
                    if (testClass != null)
                    {
                        result.Add(testClass);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Discovers test-related methods inside a single class.
        /// Returns null if the class does not contain any tests.
        /// </summary>
        private TestClassInfo? DiscoverTestClass(Type type)
        {
            var methods = type.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            var testClass = new TestClassInfo(type);

            foreach (var method in methods)
            {
                var testAttribute = method.GetCustomAttribute<TestAttribute>();
                if (testAttribute != null)
                {
                    testClass.Tests.Add(new TestMethodInfo(method, testAttribute));
                }

                if (method.GetCustomAttribute<BeforeAttribute>() != null)
                {
                    testClass.BeforeMethods.Add(method);
                }

                if (method.GetCustomAttribute<AfterAttribute>() != null)
                {
                    testClass.AfterMethods.Add(method);
                }

                if (method.GetCustomAttribute<BeforeClassAttribute>() != null)
                {
                    testClass.BeforeClassMethods.Add(method);
                }

                if (method.GetCustomAttribute<AfterClassAttribute>() != null)
                {
                    testClass.AfterClassMethods.Add(method);
                }
            }

            return testClass.Tests.Any() ? testClass : null;
        }

        /// <summary>
        /// Loads all assemblies (*.dll) from the specified directory.
        /// </summary>
        private static IEnumerable<Assembly> LoadAssemblies(string path)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(path, "*.dll"))
            {
                yield return Assembly.LoadFrom(file);
            }
        }
    }
}