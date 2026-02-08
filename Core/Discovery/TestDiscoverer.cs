// <copyright file="TestDiscoverer.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace Core.Discovery;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Attributes;
using Core.Models;

/// <summary>
/// Discovers test classes and test methods in assemblies using reflection.
/// </summary>
public class TestDiscoverer
{
    /// <summary>
    /// Safely checks whether a method has a specific attribute.
    /// </summary>
    /// <typeparam name="T">Type of attribute to check.</typeparam>
    /// <param name="method">Method to inspect.</param>
    /// <returns>True if the attribute is present; otherwise, false.</returns>
    private static bool HasAttribute<T>(MethodInfo method)
        where T : Attribute
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
    /// <typeparam name="T">Type of attribute to retrieve.</typeparam>
    /// <param name="method">Method to inspect.</param>
    /// <param name="attribute">Retrieved attribute instance.</param>
    /// <returns>True if the attribute is present; otherwise, false.</returns>
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

    /// <summary>
    /// Loads all assemblies from the specified directory.
    /// </summary>
    /// <param name="path">Path to the directory containing assemblies.</param>
    /// <returns>Enumerable of loaded assemblies.</returns>
    private static IEnumerable<Assembly> LoadAssemblies(string path)
    {
        foreach (var file in System.IO.Directory.EnumerateFiles(path, "*.dll"))
        {
            yield return Assembly.LoadFrom(file);
        }
    }

    /// <summary>
    /// Discovers test classes and test methods in assemblies using reflection.
    /// </summary>
    /// <param name="path">Path to the directory containing test assemblies.</param>
    /// <returns>List of discovered test classes.</returns>
#pragma warning disable SA1202 // Elements should be ordered by access
    public IReadOnlyList<TestClassInfo> Discover(string path)
#pragma warning restore SA1202 // Elements should be ordered by access
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
                var testClass = this.DiscoverTestClassSafe(type);
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
            return this.DiscoverTestClass(type);
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
}