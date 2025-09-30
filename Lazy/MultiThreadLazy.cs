// <copyright file="MultiThreadLazy.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;

namespace LazyImplementation
{
    /// <summary>
    /// Thread-safe implementation of ILazy{T}.
    /// Uses double-checked locking to minimize synchronization overhead.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public class MultiThreadLazy<T> : ILazy<T>
    {
        private Func<T>? _supplier;
        private T? value;
        private volatile bool isValueCreated;
        private readonly object locker = new object();

        /// <summary>
        /// Initializes a new instance of MultiThreadLazy with the given supplier.
        /// </summary>
        /// <param name="supplier">Function that will provide the value.</param>
        /// <exception cref="ArgumentNullException">Thrown if supplier is null.</exception>
        public MultiThreadLazy(Func<T> supplier)
        {
            _supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        }

        /// <summary>
        /// Returns the lazily evaluated value in a thread-safe way.
        /// - If the value is already created, returns it immediately without locking.
        /// - Otherwise, uses lock to ensure the value is created only once.
        /// </summary>
        public T Get()
        {
            if (isValueCreated)
            {
                return value;
            }

            lock (locker)
            {
                if (!isValueCreated)
                {
                    value = _supplier();
                    isValueCreated = true;
                    _supplier = null;
                }
            }

            return value;
        }
    }
}