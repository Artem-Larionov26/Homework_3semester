// <copyright file="SingleThreadLazyTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using NUnit.Framework;

namespace LazyImplementation.Tests
{
    [TestFixture]
    public class SingleThreadLazyTests
    {
        [Test]
        public void SupplierCalledOnlyOnce()
        {
            int callCount = 0;
            Func<int> supplier = () =>
            {
                callCount++;
                return 42;
            };

            var lazy = new SingleThreadLazy<int>(supplier);

            var v1 = lazy.Get();
            var v2 = lazy.Get();
            var v3 = lazy.Get();

            Assert.That(v1, Is.EqualTo(42));
            Assert.That(v2, Is.EqualTo(42));
            Assert.That(v3, Is.EqualTo(42));
            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void SupportsNullValue()
        {
            Func<string> supplier = () => null;

            var lazy = new SingleThreadLazy<string>(supplier);

            var v1 = lazy.Get();
            var v2 = lazy.Get();

            Assert.That(v1, Is.Null);
            Assert.That(v2, Is.Null);
        }

        [Test]
        public void ThrowsIfSupplierIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var lazy = new SingleThreadLazy<int>(null);
            });
        }
    }
}