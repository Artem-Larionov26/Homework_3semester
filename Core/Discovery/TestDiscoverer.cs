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
        public IReadOnlyList<TestClassInfo> Discover(string path)
        {
            var result = new List<TestClassInfo>();

            foreach (var assembly in LoadAssemblies(path))
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray()!;
                }

                foreach (var type in types)
                {
                    var testClass = DiscoverTestClassSafe(type);
                    if (testClass != null)
                    {
                        result.Add(testClass);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Safe wrapper around DiscoverTestClass.
        /// Prevents broken types from crashing discovery.
        /// </summary>
        private TestClassInfo? DiscoverTestClassSafe(Type type)
        {
            try
            {
                return DiscoverTestClass(type);
            }
            catch (TypeLoadException)
            {
                return null;
            }
        }

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
                if (HasAttribute<TestAttribute>(method, out var testAttribute))
                {
                    testClass.Tests.Add(new TestMethodInfo(method, testAttribute));
                }

                if (HasAttribute<BeforeAttribute>(method))
                {
                    testClass.BeforeMethods.Add(method);
                }

                if (HasAttribute<AfterAttribute>(method))
                {
                    testClass.AfterMethods.Add(method);
                }

                if (HasAttribute<BeforeClassAttribute>(method))
                {
                    testClass.BeforeClassMethods.Add(method);
                }

                if (HasAttribute<AfterClassAttribute>(method))
                {
                    testClass.AfterClassMethods.Add(method);
                }
            }

            return testClass.Tests.Any() ? testClass : null;
        }

        /// <summary>
        /// Safely checks whether a method has a specific attribute.
        /// </summary>
        private static bool HasAttribute<T>(MethodInfo method) where T : Attribute
        {
            try
            {
                return method.GetCustomAttribute<T>() != null;
            }
            catch (TypeLoadException)
            {
                return false;
            }
        }

        /// <summary>
        /// Safely retrieves an attribute instance if present.
        /// </summary>
        private static bool HasAttribute<T>(MethodInfo method, out T attribute)
            where T : Attribute
        {
            try
            {
                attribute = method.GetCustomAttribute<T>()!;
                return attribute != null;
            }
            catch (TypeLoadException)
            {
                attribute = null!;
                return false;
            }
        }

        private static IEnumerable<Assembly> LoadAssemblies(string path)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(path, "*.dll"))
            {
                yield return Assembly.LoadFrom(file);
            }
        }
    }
}