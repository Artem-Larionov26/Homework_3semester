// <copyright file="SingleThreadLazy.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace LazyImplementation
{
    /// <summary>
    /// Single-threaded implementation of ILazy{T}.
    /// This class isn't thread-safe and should only be used in a single-threaded environment.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class SingleThreadLazy<T> : ILazy<T>
    {
        private Func<T>? _supplier;
        private T? value;
        private bool isValueCreated;

        /// <summary>
        /// Initializes a new instance of SingleThreadLazy with the given supplier.
        /// </summary>
        /// <param name="supplier">Function that will provide the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if supplier is null.</exception>
        public SingleThreadLazy(Func<T> supplier)
        {
            _supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        }

        /// <summary>
        /// Returns the lazily evaluated value.
        /// - On the first call, supplier() is executed and the result is cached.
        /// - On subsequent calls, the cached value is returned.
        /// - Supplier is set to null after first use, allowing it to be garbage-collected.
        /// </summary>
        public T Get()
        {
            if (!isValueCreated)
            {
                value = _supplier();
                isValueCreated = true;
                _supplier = null;
            }

            return value;
        }
    }
}