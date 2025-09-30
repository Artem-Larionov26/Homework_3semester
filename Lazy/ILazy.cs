// <copyright file="ILazy.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

namespace LazyImplementation
{
    /// <summary>
    /// Represents a lazily evaluated value of type T.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Returns the lazily calculated value.
        /// </summary>
        /// <returns>The calculated value of type T.</returns>
        T Get();
    }
}