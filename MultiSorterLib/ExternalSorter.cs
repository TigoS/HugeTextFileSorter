namespace MultiSorterLib
{
    /// <summary>
    /// Provides methods for performing external sorting operations on large text files that may not fit entirely in
    /// memory.
    /// </summary>
    /// <remarks>The ExternalSorter class is designed for scenarios where input files are too large to be
    /// sorted in memory. It splits the input file into manageable, sorted chunks that can later be merged to produce a
    /// fully sorted file. All members of this class are static and thread-safe for independent operations.</remarks>
    public static class ExternalSorter
    {
        /// <summary>
        /// Represents the default chunk size, in megabytes, used for data processing or storage operations.
        /// </summary>
        const int CHUNK_SIZE_MB = 256;

        /// <summary>
        /// Represents the multiplier used to calculate chunk sizes in bytes.
        /// </summary>
        /// <remarks>This constant is typically used to convert values to kilobytes when determining
        /// buffer or chunk sizes.</remarks>
        const long CHUNK_SIZE_MULTIPLIER = 1024L;

        /// <summary>
        /// Splits a large input file into multiple sorted temporary chunk files.
        /// </summary>
        /// <remarks>Each temporary file contains a sorted subset of lines from the input file, using
        /// ordinal string comparison. The caller is responsible for deleting the temporary files after use. This method
        /// is useful for external sorting of large files that do not fit into memory.</remarks>
        /// <param name="inputFile">The path to the input file to be split and sorted. The file must exist and be accessible for reading.</param>
        /// <returns>A list of file paths to the temporary files, each containing a sorted chunk of the original input. The list
        /// will be empty if the input file contains no non-empty lines.</returns>
        public static List<string> CreateSortedChunks(string inputFile)
        {
            
            var tempFiles = new List<string>();
            long maxBytes = CHUNK_SIZE_MB * CHUNK_SIZE_MULTIPLIER * CHUNK_SIZE_MULTIPLIER;

            using var reader = new StreamReader(inputFile);
            if (reader != null)
            {
                List<string> lines;
                long currentBytes;
                string line, tempFile;

                while (!reader.EndOfStream)
                {
                    lines = new List<string>();
                    currentBytes = 0;

                    while (!reader.EndOfStream && currentBytes < maxBytes)
                    {
                        line = reader.ReadLine() ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            lines.Add(line);
                            currentBytes += line.Length * sizeof(char);
                        }
                    }

                    lines.Sort(StringComparer.Ordinal);

                    tempFile = Path.GetTempFileName();
                    File.WriteAllLines(tempFile, lines);
                    tempFiles.Add(tempFile);
                }
            }

            return tempFiles;
        }
    }
}
