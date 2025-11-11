// <copyright file="HashExtensions.cs" company="Larionov Artem">
// Copyright (c) Larionov Artem. All rights reserved.
// </copyright>

using System.Text;

namespace DirectoryChecksum.Utilities
{
    /// <summary>
    /// Utilities for working with hashes and byte arrays
    /// </summary>
    public static class HashExtensions
    {
        /// <summary>
        /// Converts byte array to hex string
        /// </summary>
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Concatenates multiple byte arrays
        /// </summary>
        public static byte[] Concatenate(this byte[] first, params byte[][] arrays)
        {
            var totalLength = first.Length + arrays.Sum(a => a.Length);
            var result = new byte[totalLength];

            // Copy first array
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            var offset = first.Length;

            // Copy other arrays
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        /// <summary>
        /// Compares two byte arrays for equality
        /// </summary>
        public static bool SequenceEqual(this byte[] first, byte[] second)
        {
            if (first == null || second == null)
                return first == second;

            if (first.Length != second.Length)
                return false;

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                    return false;
            }

            return true;
        }
    }
}