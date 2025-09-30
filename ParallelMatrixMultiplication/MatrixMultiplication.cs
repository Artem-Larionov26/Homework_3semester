// <copyright file="MatrixMultiplication.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace ParallelMatrixMultiplication
{
    /// <summary>
    /// A class for performing sequential matrix multiplication.
    /// </summary>
    public static class MatrixMultiplication
    {
        /// <summary>
        /// Sequential multiplication of two integer matrices.
        /// </summary>
        /// <param name="A">The left matrix is of size n × m.</param>
        /// <param name="B">The right matrix is of size m × k.</param>
        /// <returns>A new matrix of size n × k is the result of A × B.</returns>
        /// <exception cref="ArgumentNullException">If A or B are null.</exception>
        /// <exception cref="ArgumentException">If the number of columns of A is not equal to the number of rows of B.</exception>
        public static int[,] MultiplySequential(int[,] A, int[,] B)
        {
            if (A == null)
            {
                throw new ArgumentNullException(nameof(A));
            }
            if (B == null)
            {
                throw new ArgumentNullException(nameof(B));
            }

            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            int colsB = B.GetLength(1);

            if (colsA != rowsB)
            {
                throw new ArgumentException("The number of columns of matrix A must be equal to the number of rows of matrix B.");
            }

            int[,] C = new int[rowsA, colsB];
            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    int sum = 0;
                    for (int r = 0; r < colsA; r++)
                    {
                        sum += A[i, r] * B[r, j];
                    }
                    C[i, j] = sum;
                }
            }

            return C;
        }

        /// <summary>
        /// Multiplies two matrices in parallel using a limited number of threads.
        /// Unlike MultiplyParallelThread, which creates one thread per row (may be too many),
        /// this method creates only as many threads as there are processor cores
        /// and distributes the work between them.
        /// </summary>
        /// <param name="A">Matrix A (left operand).</param>
        /// <param name="B">Matrix B (right operand).</param>
        /// <returns>The resulting matrix C = A × B.</returns>
        /// <exception cref="ArgumentException">Thrown if the matrices are incompatible for multiplication.</exception>
        public static int[,] MultiplyParallelThreadLimited(int[,] A, int[,] B)
        {
            int rowsA = A.GetLength(0);
            int colsA = A.GetLength(1);
            int rowsB = B.GetLength(0);
            int colsB = B.GetLength(1);

            if (colsA != rowsB)
            {
                throw new ArgumentException("Matrix dimensions are not compatible for multiplication!");
            }

            int[,] result = new int[rowsA, colsB];

            int threadCount = Environment.ProcessorCount;

            int chunkSize = (int)Math.Ceiling((double)rowsA / threadCount);

            Thread[] threads = new Thread[threadCount];

            for (int t = 0; t < threadCount; t++)
            {
                int startRow = t * chunkSize;
                int endRow = Math.Min(startRow + chunkSize, rowsA);

                threads[t] = new Thread(() =>
                {
                    for (int i = startRow; i < endRow; i++)
                    {
                        for (int j = 0; j < colsB; j++)
                        {
                            int sum = 0;
                            for (int k = 0; k < colsA; k++)
                            {
                                sum += A[i, k] * B[k, j];
                            }
                            result[i, j] = sum;
                        }
                    }
                });

                threads[t].Start();
            }

            foreach (var thread in threads)
                thread.Join();

            return result;
        }

        /// <summary>
        /// Output of the matrix to the console.
        /// </summary>
        /// <param name="M">The matrix to output.</param>
        /// <param name="name">Matrix Name.</param>
        public static void PrintMatrix(int[,] M, string name = "M")
        {
            if (M == null)
            {
                Console.WriteLine($"{name} = null");
                return;
            }

            int rows = M.GetLength(0);
            int cols = M.GetLength(1);
            Console.WriteLine($"{name} ({rows}x{cols}):");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{M[i, j],6}");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Compares two integer matrices element by element.
        /// </summary>
        /// <param name="A">The first matrix.</param>
        /// <param name="B">The second matrix.</param>
        /// <returns>
        /// <c>true</c> if the matrices have the same size and identical values in all positions;  
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if either <paramref name="A"/> or <paramref name="B"/> is <c>null</c>.
        /// </exception>
        public static bool CompareMatrices(int[,] A, int[,] B)
        {
            if (A == null)
            {
                throw new ArgumentNullException(nameof(A));
            }
            if (B == null)
            {
                throw new ArgumentNullException(nameof(B));
            }
            if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    if (A[i, j] != B[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// A simple check of the equality of two matrices.
        /// </summary>
        /// <param name="A">The first matrix.</param>
        /// <param name="B">The second matrix.</param>
        /// <returns>
        /// true, if the matrices are equal in size and content; else false.
        /// </returns>
        public static bool AreEqual(int[,] A, int[,] B)
        {
            if (A == null || B == null)
            {
                return A == B;
            }
            if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            {
                return false;
            }

            int rows = A.GetLength(0);
            int cols = A.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (A[i, j] != B[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    class Program
    {
        static void Main()
        {
            int[,] A = new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            int[,] B = new int[,] { { 7, 8 }, { 9, 10 }, { 11, 12 } };

            MatrixMultiplication.PrintMatrix(A, "A");
            MatrixMultiplication.PrintMatrix(B, "B");

            int[,] C = MatrixMultiplication.MultiplySequential(A, B);

            MatrixMultiplication.PrintMatrix(C, "A x B (consistently)");

            int[,] C2 = MatrixMultiplication.MultiplyParallelThreadLimited(A, B);

            MatrixMultiplication.PrintMatrix(C2, "A × B (parallel)");

            Console.WriteLine("Comparison: " +
                (MatrixMultiplication.AreEqual(C, C2) ? "OK" : "FAIL"));
        }
    }
}