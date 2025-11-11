// <copyright file="DirectoryHasher.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Security.Cryptography;
using System.Text;
using DirectoryChecksum.Utilities;

namespace DirectoryChecksum.Core
{
    /// <summary>
    /// Computes checksums for file system directories
    /// </summary>
    public class DirectoryHasher
    {
        /// <summary>
        /// Computes MD5 hash for a byte array
        /// </summary>
        private byte[] ComputeMD5(byte[] data)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(data);
        }

        /// <summary>
        /// Single-threaded file checksum calculation
        /// </summary>
        public byte[] CalculateFileHash(string filePath)
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                var fileContentBytes = File.ReadAllBytes(filePath);

                var dataToHash = fileNameBytes.Concatenate(fileContentBytes);
                return ComputeMD5(dataToHash);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to file: {filePath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Single-threaded directory checksum calculation
        /// </summary>
        public byte[] CalculateDirectoryHashSingleThreaded(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            var directoryName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));
            var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

            var entries = GetSortedEntries(directoryPath);

            if (entries.Length == 0)
            {
                return ComputeMD5(directoryNameBytes);
            }

            var allHashes = new List<byte[]>();

            foreach (var entry in entries)
            {
                byte[] entryHash;

                if (Directory.Exists(entry))
                {
                    entryHash = CalculateDirectoryHashSingleThreaded(entry);
                }
                else
                {
                    entryHash = CalculateFileHash(entry);
                }

                allHashes.Add(entryHash);
            }

            var dataToHash = new List<byte[]>();
            dataToHash.Add(directoryNameBytes);
            dataToHash.AddRange(allHashes);

            return ComputeMD5(ConcatenateArrays(dataToHash.ToArray()));
        }

        /// <summary>
        /// Multi-threaded directory checksum calculation
        /// </summary>
        public byte[] CalculateDirectoryHashMultiThreaded(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var directoryName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));
            var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

            var entries = GetSortedEntries(directoryPath);

            if (entries.Length == 0)
            {
                return ComputeMD5(directoryNameBytes);
            }

            var tasks = new List<Task<byte[]>>();

            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    var task = Task.Run(() => CalculateDirectoryHashMultiThreaded(entry));
                    tasks.Add(task);
                }
                else
                {
                    var task = Task.Run(() => CalculateFileHash(entry));
                    tasks.Add(task);
                }
            }

            Task.WaitAll(tasks.ToArray());

            var allHashes = tasks.Select(task => task.Result).ToList();

            var dataToHash = new List<byte[]>();
            dataToHash.Add(directoryNameBytes);
            dataToHash.AddRange(allHashes);

            return ComputeMD5(ConcatenateArrays(dataToHash.ToArray()));
        }

        /// <summary>
        /// Asynchronous file checksum calculation
        /// </summary>
        public async Task<byte[]> CalculateFileHashAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
                byte[] fileContentBytes;

                using (var fileStream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true))
                {
                    fileContentBytes = new byte[fileStream.Length];
                    await fileStream.ReadAsync(fileContentBytes, 0, (int)fileStream.Length);
                }

                var dataToHash = fileNameBytes.Concatenate(fileContentBytes);
                return ComputeMD5(dataToHash);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to file: {filePath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error reading file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Asynchronous directory checksum calculation
        /// </summary>
        public async Task<byte[]> CalculateDirectoryHashAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var directoryName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));
            var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

            var entries = GetSortedEntries(directoryPath);

            if (entries.Length == 0)
            {
                return ComputeMD5(directoryNameBytes);
            }

            var tasks = new List<Task<byte[]>>();

            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    var task = CalculateDirectoryHashAsync(entry);
                    tasks.Add(task);
                }
                else
                {
                    var task = CalculateFileHashAsync(entry);
                    tasks.Add(task);
                }
            }

            var allHashes = await Task.WhenAll(tasks);

            var dataToHash = new List<byte[]>();
            dataToHash.Add(directoryNameBytes);
            dataToHash.AddRange(allHashes);

            return ComputeMD5(ConcatenateArrays(dataToHash.ToArray()));
        }

        /// <summary>
        /// Hybrid version - async file reading with parallel processing
        /// </summary>
        public async Task<byte[]> CalculateDirectoryHashHybridAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var directoryName = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar));
            var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

            var entries = GetSortedEntries(directoryPath);

            if (entries.Length == 0)
            {
                return ComputeMD5(directoryNameBytes);
            }

            var tasks = new List<Task<byte[]>>();

            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                {
                    var task = CalculateDirectoryHashHybridAsync(entry);
                    tasks.Add(task);
                }
                else
                {
                    var task = Task.Run(async () =>
                    {
                        var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(entry));
                        byte[] fileContentBytes;

                        using (var fileStream = new FileStream(
                            entry,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            bufferSize: 4096,
                            useAsync: true))
                        {
                            fileContentBytes = new byte[fileStream.Length];
                            await fileStream.ReadAsync(fileContentBytes, 0, (int)fileStream.Length);
                        }

                        var dataToHash = fileNameBytes.Concatenate(fileContentBytes);
                        return ComputeMD5(dataToHash);
                    });
                    tasks.Add(task);
                }
            }

            var allHashes = await Task.WhenAll(tasks);

            var dataToHash = new List<byte[]>();
            dataToHash.Add(directoryNameBytes);
            dataToHash.AddRange(allHashes);

            return ComputeMD5(ConcatenateArrays(dataToHash.ToArray()));
        }

        /// <summary>
        /// Gets sorted list of files and subdirectories
        /// </summary>
        private string[] GetSortedEntries(string directoryPath)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath);
                var directories = Directory.GetDirectories(directoryPath);

                return files.Concat(directories)
                           .OrderBy(path => path, StringComparer.Ordinal)
                           .ToArray();
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Access denied to directory: {directoryPath}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Error accessing directory: {directoryPath}", ex);
            }
        }

        /// <summary>
        /// Concatenates multiple byte arrays (helper method)
        /// </summary>
        private byte[] ConcatenateArrays(params byte[][] arrays)
        {
            if (arrays.Length == 0)
            {
                return Array.Empty<byte>();
            }

            if (arrays.Length == 1)
            {
                return arrays[0];
            }

            var totalLength = arrays.Sum(a => a.Length);
            var result = new byte[totalLength];
            var offset = 0;

            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        /// <summary>
        /// Converts byte array to readable hex string
        /// </summary>
        public string HashToHexString(byte[] hash)
        {
            return hash.ToHexString();
        }

        /// <summary>
        /// Compares two hashes for equality
        /// </summary>
        public bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            return hash1.SequenceEqual(hash2);
        }
    }
}