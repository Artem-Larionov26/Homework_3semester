// <copyright file="UnitTest1.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using NUnit.Framework;
using System.IO;
using DirectoryChecksum.Core;
using DirectoryChecksum.Utilities;

namespace DirectoryChecksum.Tests
{
    [TestFixture]
    public class DirectoryHasherTests
    {
        private string _testDirectory;
        private DirectoryHasher _hasher;

        [SetUp]
        public void SetUp()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "DirectoryChecksumTest_" + Path.GetRandomFileName());
            _hasher = new DirectoryHasher();

            SafeDeleteDirectory(_testDirectory);

            Directory.CreateDirectory(_testDirectory);

            File.WriteAllText(Path.Combine(_testDirectory, "file1.txt"), "Hello World!");
            File.WriteAllText(Path.Combine(_testDirectory, "file2.txt"), "Another test file");

            var subDir = Path.Combine(_testDirectory, "subdir");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "subfile1.txt"), "Subdirectory file content");
            File.WriteAllText(Path.Combine(subDir, "subfile2.txt"), "More content");

            var subSubDir = Path.Combine(subDir, "subsubdir");
            Directory.CreateDirectory(subSubDir);
            File.WriteAllText(Path.Combine(subSubDir, "deepfile.txt"), "Deep file content");
        }

        [TearDown]
        public void TearDown()
        {
            SafeDeleteDirectory(_testDirectory);
        }

        /// <summary>
        /// Safely delete directory with retry logic for cross-platform compatibility
        /// </summary>
        private void SafeDeleteDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                    break;
                }
                catch (IOException) when (i < 2)
                {
                    System.Threading.Thread.Sleep(100);
                }
                catch (UnauthorizedAccessException) when (i < 2)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        [Test]
        public void CalculateFileHash_ValidFile_ReturnsCorrectHash()
        {
            var filePath = Path.Combine(_testDirectory, "file1.txt");

            var hash = _hasher.CalculateFileHash(filePath);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public void CalculateFileHash_NonExistentFile_ThrowsFileNotFoundException()
        {
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

            Assert.Throws<FileNotFoundException>(() => _hasher.CalculateFileHash(nonExistentFile));
        }

        [Test]
        public void CalculateDirectoryHashSingleThreaded_ValidDirectory_ReturnsHash()
        {
            var hash = _hasher.CalculateDirectoryHashSingleThreaded(_testDirectory);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public void CalculateDirectoryHashMultiThreaded_ValidDirectory_ReturnsHash()
        {
            var hash = _hasher.CalculateDirectoryHashMultiThreaded(_testDirectory);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public void SingleThreadedAndMultiThreaded_ReturnSameHash()
        {
            var singleThreadedHash = _hasher.CalculateDirectoryHashSingleThreaded(_testDirectory);
            var multiThreadedHash = _hasher.CalculateDirectoryHashMultiThreaded(_testDirectory);

            Assert.That(_hasher.CompareHashes(singleThreadedHash, multiThreadedHash), Is.True);
        }

        [Test]
        public void CalculateDirectoryHash_EmptyDirectory_ReturnsHash()
        {
            var emptyDir = Path.Combine(_testDirectory, "empty");
            Directory.CreateDirectory(emptyDir);

            var hash = _hasher.CalculateDirectoryHashSingleThreaded(emptyDir);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public void CalculateDirectoryHash_NonExistentDirectory_ThrowsDirectoryNotFoundException()
        {
            var nonExistentDir = Path.Combine(_testDirectory, "nonexistent");

            Assert.Throws<DirectoryNotFoundException>(() => _hasher.CalculateDirectoryHashSingleThreaded(nonExistentDir));
        }

        [Test]
        public void HashToHexString_ConvertsCorrectly()
        {
            byte[] testBytes = { 0xAB, 0xCD, 0xEF, 0x12, 0x34 };

            var hex = _hasher.HashToHexString(testBytes);

            Assert.That(hex, Is.EqualTo("abcdef1234"));
        }

        [Test]
        public void CompareHashes_EqualHashes_ReturnsTrue()
        {
            byte[] hash1 = { 0x01, 0x02, 0x03 };
            byte[] hash2 = { 0x01, 0x02, 0x03 };

            Assert.That(_hasher.CompareHashes(hash1, hash2), Is.True);
        }

        [Test]
        public void CompareHashes_DifferentHashes_ReturnsFalse()
        {
            byte[] hash1 = { 0x01, 0x02, 0x03 };
            byte[] hash2 = { 0x01, 0x02, 0x04 };

            Assert.That(_hasher.CompareHashes(hash1, hash2), Is.False);
        }

        [Test]
        public async Task CalculateFileHashAsync_ValidFile_ReturnsCorrectHash()
        {
            var filePath = Path.Combine(_testDirectory, "file1.txt");

            var hash = await _hasher.CalculateFileHashAsync(filePath);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public async Task CalculateDirectoryHashAsync_ValidDirectory_ReturnsHash()
        {
            var hash = await _hasher.CalculateDirectoryHashAsync(_testDirectory);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public async Task AsyncAndSynchronous_ReturnSameHash()
        {
            var synchronousHash = _hasher.CalculateDirectoryHashSingleThreaded(_testDirectory);
            var asyncHash = await _hasher.CalculateDirectoryHashAsync(_testDirectory);

            Assert.That(_hasher.CompareHashes(synchronousHash, asyncHash), Is.True);
        }

        [Test]
        public async Task HybridAsync_ValidDirectory_ReturnsHash()
        {
            var hash = await _hasher.CalculateDirectoryHashHybridAsync(_testDirectory);

            Assert.That(hash, Is.Not.Null);
            Assert.That(hash.Length, Is.EqualTo(16));
        }

        [Test]
        public void AllHashMethods_ReturnSameHash()
        {
            var singleThreadedHash = _hasher.CalculateDirectoryHashSingleThreaded(_testDirectory);
            var multiThreadedHash = _hasher.CalculateDirectoryHashMultiThreaded(_testDirectory);
            var asyncHash = _hasher.CalculateDirectoryHashAsync(_testDirectory).Result;
            var hybridHash = _hasher.CalculateDirectoryHashHybridAsync(_testDirectory).Result;

            Assert.That(_hasher.CompareHashes(singleThreadedHash, multiThreadedHash), Is.True);
            Assert.That(_hasher.CompareHashes(singleThreadedHash, asyncHash), Is.True);
            Assert.That(_hasher.CompareHashes(singleThreadedHash, hybridHash), Is.True);
        }
    }
}