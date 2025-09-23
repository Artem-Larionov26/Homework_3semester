// <copyright file="MatrixUserInterface.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace ParallelMatrixMultiplication
{
    /// <summary>
    /// Methods for working with files: reading and writing matrices.
    /// </summary>
    public static class MatrixUserInterface
    {
        /// <summary>
        /// Generating a random integer matrix and writing to a file.
        /// </summary>
        /// <param name="rows">Number of rows.</param>
        /// <param name="cols">Number of columns.</param>
        /// <param name="minValue">Minimum value of the element.</param>
        /// <param name="maxValue">Maximum value of the element (not including).</param>
        /// <param name="filePath">File path.</param>
        public static void GenerateRandomMatrix(int rows, int cols, int minValue, int maxValue, string filePath)
        {
            Random random = new Random();
            int[,] matrix = new int[rows, cols];

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    string[] row = new string[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        int value = random.Next(minValue, maxValue);
                        matrix[i, j] = value;
                        row[j] = value.ToString();
                    }
                    writer.WriteLine(string.Join(" ", row));
                }
            }
        }

        /// <summary>
        /// Generates a random integer matrix of the specified size and value range in memory.
        /// </summary>
        /// <param name="rows">The number of rows of the matrix.</param>
        /// <param name="cols">The number of columns of the matrix.</param>
        /// <param name="min">The minimum possible value of the elements (inclusive).</param>
        /// <param name="max">The maximum possible value of the elements (inclusive).</param>
        /// <returns>
        /// A two-dimensional integer array of size <paramref name="rows"/> × <paramref name="cols"/> 
        /// filled with random integers between <paramref name="min"/> and <paramref name="max"/>.
        /// </returns>
        /// <remarks>
        /// This method does not write the matrix to a file, unlike <c>GenerateRandomMatrix</c>.  
        /// It is useful for testing algorithms directly in memory.
        /// </remarks>
        public static int[,] GenerateRandomMatrixInMemory(int rows, int cols, int min, int max)
        {
            var rand = new Random();
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = rand.Next(min, max + 1);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Reading a matrix from a text file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>A matrix in the form of a two-dimensional array.</returns>
        public static int[,] ReadMatrix(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не найден.");
            }

            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                throw new ArgumentException("Файл пуст, матрицу считать нельзя.");
            }

            int rows = lines.Length;
            int cols = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            int[,] matrix = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] parts = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != cols)
                {
                    throw new FormatException("Нарушена прямоугольность матрицы в файле.");
                }

                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = int.Parse(parts[j]);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Writing the matrix to a text file.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="filePath">File path.</param>
        public static void WriteMatrix(int[,] matrix, string filePath)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    string[] row = new string[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        row[j] = matrix[i, j].ToString();
                    }
                    writer.WriteLine(string.Join(" ", row));
                }
            }
        }
    }
}