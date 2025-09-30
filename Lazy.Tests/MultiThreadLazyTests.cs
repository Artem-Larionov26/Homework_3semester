// <copyright file="MultiThreadLazyTests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace LazyImplementation.Tests
{
    [TestFixture]
    public class MultiThreadLazyTests
    {
        [Test]
        public void SupplierCalledOnlyOnce()
        {
            int callCount = 0;
            Func<int> supplier = () =>
            {
                Interlocked.Increment(ref callCount);
                return 99;
            };

            var lazy = new MultiThreadLazy<int>(supplier);

            var v1 = lazy.Get();
            var v2 = lazy.Get();
            var v3 = lazy.Get();

            Assert.That(v1, Is.EqualTo(99));
            Assert.That(v2, Is.EqualTo(99));
            Assert.That(v3, Is.EqualTo(99));
            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void SupportsNullValue()
        {
            Func<string> supplier = () => null;

            var lazy = new MultiThreadLazy<string>(supplier);

            var v1 = lazy.Get();
            var v2 = lazy.Get();

            Assert.That(v1, Is.Null);
            Assert.That(v2, Is.Null);
        }

        [Test]
        public void ShouldBeThreadSafe()
        {
            int callCount = 0;
            Func<int> supplier = () =>
            {
                Interlocked.Increment(ref callCount);
                Thread.Sleep(50);
                return 777;
            };

            var lazy = new MultiThreadLazy<int>(supplier);
            int[] results = new int[10];

            Parallel.For(0, 10, i =>
            {
                results[i] = lazy.Get();
            });

            foreach (var result in results)
                Assert.That(result, Is.EqualTo(777));

            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void ThrowsIfSupplierIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var lazy = new MultiThreadLazy<int>(null);
            });
        }
    }
}