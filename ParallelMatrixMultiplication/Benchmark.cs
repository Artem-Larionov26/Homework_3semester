// <copyright file="Benchmark.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace ParallelMatrixMultiplication
{
    /// <summary>
    /// A class for measuring the execution time of matrix multiplication algorithms.
    /// It runs the selected algorithm multiple times, calculates the average execution time
    /// (mathematical expectation) and the standard deviation (dispersion of measurements).
    /// </summary>
    public static class Benchmark
    {
        /// <summary>
        /// Runs the specified multiplication method N times, measures execution time,
        /// and calculates statistics (mean and standard deviation).
        /// </summary>
        /// <param name="multiplyMethod">
        /// A function that multiplies two matrices and returns the result.
        /// </param>
        /// <param name="A">Left matrix.</param>
        /// <param name="B">Right matrix.</param>
        /// <param name="runs">Number of runs for statistical measurement.</param>
        /// <returns>
        /// Tuple: (mean execution time in ms, standard deviation in ms).
        /// </returns>
        public static (double mean, double stdDev) Run(
            Func<int[,], int[,], int[,]> multiplyMethod,
            int[,] A,
            int[,] B,
            int runs = 5)
        {
            if (multiplyMethod == null)
            {
                throw new ArgumentNullException(nameof(multiplyMethod));
            }
            if (A == null || B == null)
            {
                throw new ArgumentNullException("Matrices cannot be null");
            }

            double[] times = new double[runs];
            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < runs; i++)
            {
                sw.Restart();
                multiplyMethod(A, B);
                sw.Stop();
                times[i] = sw.Elapsed.TotalMilliseconds;
            }

            double mean = 0;
            foreach (double t in times)
                mean += t;
            mean /= runs;

            double variance = 0;
            foreach (double t in times)
                variance += Math.Pow(t - mean, 2);
            variance /= runs;
            double stdDev = Math.Sqrt(variance);

            return (mean, stdDev);
        }
    }
}
