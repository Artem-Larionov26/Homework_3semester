// <copyright file="Program.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using ParallelMatrixMultiplication;
using System.Diagnostics;

Console.WriteLine("Parallel matrix multiplication\n");
Console.WriteLine(
    "Enter a number for further action:\n" +
    "1. Multiply matrices from files\n" +
    "2. Generate random matrices and save to files\n" +
    "3. Run benchmark (compare sequential vs parallel)\n" +
    "0. Exit\n");
Console.Write("Enter number: ");

string? choice = Console.ReadLine();

try
{
    switch (choice)
    {
        case "1":
            Console.Write("Enter path to file with matrix A: ");
            string fileA = Console.ReadLine() ?? throw new ArgumentException("Path A is empty");

            Console.Write("Enter path to file with matrix B: ");
            string fileB = Console.ReadLine() ?? throw new ArgumentException("Path B is empty");

            int[,] A = MatrixUserInterface.ReadMatrix(fileA);
            int[,] B = MatrixUserInterface.ReadMatrix(fileB);

            Console.WriteLine("Matrix A:");
            MatrixMultiplication.PrintMatrix(A, "A");

            Console.WriteLine("Matrix B:");
            MatrixMultiplication.PrintMatrix(B, "B");

            var sw = Stopwatch.StartNew();
            int[,] C_seq = MatrixMultiplication.MultiplySequential(A, B);
            sw.Stop();
            Console.WriteLine($"Sequential multiplication time: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("Result (A × B) [sequential]:");
            MatrixMultiplication.PrintMatrix(C_seq, "C_seq");

            sw.Restart();
            int[,] C_thr = MatrixMultiplication.MultiplyParallelThreadLimited(A, B);
            sw.Stop();
            Console.WriteLine($"Parallel (Thread) multiplication time: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("Result (A × B) [parallel threads]:");
            MatrixMultiplication.PrintMatrix(C_thr, "C_thr");

            if (MatrixMultiplication.AreEqual(C_seq, C_thr))
            {
                Console.WriteLine("Sequential and parallel results match!\n");
            }
            else
            {
                Console.WriteLine("Results differ! Check implementation.\n");
            }

            Console.Write("Enter path to save result matrix: ");
            string fileOut = Console.ReadLine() ?? "result.txt";
            MatrixUserInterface.WriteMatrix(C_seq, fileOut);

            Console.WriteLine($"Result saved to {fileOut}");
            break;

        case "2":
            Console.Write("Enter number of rows for matrix A: ");
            int rowsA = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter number of cols for matrix A: ");
            int colsA = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter number of rows for matrix B: ");
            int rowsB = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter number of cols for matrix B: ");
            int colsB = int.Parse(Console.ReadLine() ?? "0");

            if (colsA != rowsB)
            {
                throw new ArgumentException("Matrix dimensions do not allow multiplication!");
            }

            MatrixUserInterface.GenerateRandomMatrix(rowsA, colsA, -10, 10, "matrixA.txt");
            MatrixUserInterface.GenerateRandomMatrix(rowsB, colsB, -10, 10, "matrixB.txt");

            Console.WriteLine("Matrices generated and saved to matrixA.txt and matrixB.txt");
            break;

        case "3":
            Console.WriteLine("\nRunning benchmark...");
            BenchmarkRunner.Run();
            break;

        case "0":
            Console.WriteLine("Exit...");
            break;

        default:
            Console.WriteLine("Unknown operation number");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}