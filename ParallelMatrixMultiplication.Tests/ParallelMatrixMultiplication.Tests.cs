// <copyright file="ParallelMatrixMultiplication.Tests.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using System;
using System.IO;

namespace ParallelMatrixMultiplication.Tests
{
    [TestFixture]
    public class MatrixTests
    {
        [Test]
        public void MultiplySequential_SmallMatrices_ReturnsCorrectResult()
        {
            int[,] A = { { 1, 2 }, { 3, 4 } };
            int[,] B = { { 5, 6 }, { 7, 8 } };

            int[,] expected = { { 19, 22 }, { 43, 50 } };

            var result = MatrixMultiplication.MultiplySequential(A, B);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void MultiplyParallelThreadLimited_SmallMatrices_ReturnsCorrectResult()
        {
            int[,] A = { { 1, 2 }, { 3, 4 } };
            int[,] B = { { 5, 6 }, { 7, 8 } };

            int[,] expected = { { 19, 22 }, { 43, 50 } };

            var result = MatrixMultiplication.MultiplyParallelThreadLimited(A, B);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParallelAndSequentialResults_AreEqual()
        {
            int[,] A = { { 1, 2, 3 }, { 4, 5, 6 } };
            int[,] B = { { 7, 8 }, { 9, 10 }, { 11, 12 } };

            var seq = MatrixMultiplication.MultiplySequential(A, B);
            var par = MatrixMultiplication.MultiplyParallelThreadLimited(A, B);

            Assert.That(MatrixMultiplication.AreEqual(seq, par), Is.True);
        }

        [Test]
        public void Multiply_InvalidDimensions_ThrowsException()
        {
            int[,] A = { { 1, 2 }, { 3, 4 } };
            int[,] B = { { 5, 6, 7 } };

            Assert.Throws<ArgumentException>(() =>
                MatrixMultiplication.MultiplySequential(A, B));

            Assert.Throws<ArgumentException>(() =>
                MatrixMultiplication.MultiplyParallelThreadLimited(A, B));
        }

        [Test]
        public void WriteAndReadMatrix_ReturnsSameMatrix()
        {
            int[,] matrix = { { 1, 2, 3 }, { 4, 5, 6 } };
            string path = Path.GetTempFileName();

            try
            {
                MatrixUserInterface.WriteMatrix(matrix, path);
                var read = MatrixUserInterface.ReadMatrix(path);

                Assert.That(MatrixMultiplication.AreEqual(matrix, read), Is.True);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public void GenerateRandomMatrixInMemory_CreatesMatrixOfCorrectSize()
        {
            var matrix = MatrixUserInterface.GenerateRandomMatrixInMemory(3, 4, -5, 5);

            Assert.That(matrix.GetLength(0), Is.EqualTo(3));
            Assert.That(matrix.GetLength(1), Is.EqualTo(4));
        }
    }
}

