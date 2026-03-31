using MultiSorterLib;
using System.Text;
using static MultiSorterLib.FileSizeExtensions;

namespace TestFileGenerator
{
    /// <summary>
    /// Provides methods for generating test files containing random or partially duplicated alphanumeric data for use in testing and benchmarking scenarios.
    /// </summary>
    /// <remarks>This class is static and cannot be instantiated. It is designed to facilitate the creation of large files with customizable content patterns, such as varying word counts, word lengths, and duplicate string densities. All members are thread-safe for concurrent use.</remarks>
    public static class RandomFileGenerator
    {
        private const int One = 1;
        private const int Hundred = 100;
        private const string Space = " ";
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyz";

        private static readonly Random random = new();
        
        /// <summary>
        /// Generates a text file containing random or duplicate lines according to the specified parameters and returns the number of lines written.
        /// </summary>
        /// <remarks>If the operation is cancelled via the cancellation token, an
        /// OperationCanceledException is thrown. The method attempts to match the requested file size as closely as possible, but the actual size may differ slightly due to line length variations. The generated file will be overwritten if it already exists.</remarks>
        /// <param name="fileName">The path and name of the file to create. If the file already exists, it will be overwritten.</param>
        /// <param name="fileSize">The desired size of the generated file, in bytes. The method attempts to create a file as close as possible to this size.</param>
        /// <param name="maxNumber">The maximum number value to include in generated lines. Defaults to Int32.MaxValue.</param>
        /// <param name="maxWordsCount">The maximum number of words per line. Each line will contain up to this many words. Defaults to 150.</param>
        /// <param name="maxWordLength">The maximum length, in characters, of each word in a line. Defaults to 20.</param>
        /// <param name="duplicateStringDensity">The percentage (0–100) of lines that should be duplicates. A value of 0 means all lines are unique; 100 means all lines are duplicates. Defaults to 0.</param>
        /// <param name="cts">A cancellation token that can be used to cancel the file generation operation.</param>
        /// <returns>The number of lines written to the generated file, or -1 if the file could not be created.</returns>
        public static long GenerateTestFile(
            string fileName,
            ulong fileSize,
            int maxNumber = int.MaxValue,
            int maxWordsCount = 150,
            int maxWordLength = 20,
            short duplicateStringDensity = 0,
            CancellationToken cts = default)
        {
            // Creating an empty file to be able to track it's size change via 'FileInfo'
            var fileStream = File.Create(fileName);
            fileStream.Close();
            fileStream.Dispose();

            if (!File.Exists(fileName))
            {
                return -1;
            }

            long generatedLinesCount = 0;
            StringBuilder sb = new StringBuilder();
            FileInfo fileInfo = new FileInfo(fileName);
            string[] duplicateStringsBuffer = new string[Hundred - duplicateStringDensity + 1];

            // Preparing duplicate string buffer where strings should be the same and only number parts may differ
            if (duplicateStringDensity > decimal.Zero)
            {
                for (int i = 0; i < duplicateStringsBuffer.Length; i++)
                {
                    duplicateStringsBuffer[i] = GetRandomString(maxWordsCount, maxWordLength);
                }
            }

            while (fileInfo.Length < (long)(fileSize - ushort.MaxValue))
            {
                if (cts.IsCancellationRequested)
                {
                    cts.ThrowIfCancellationRequested();
                }

                generatedLinesCount++;

                if (sb.Length >= ushort.MaxValue)
                {
                    File.AppendAllText(fileName, sb.ToString());
                    sb.Clear();

                    fileInfo = new FileInfo(fileName);
                }

                if (duplicateStringDensity >= Hundred)
                {
                    // All strings should be the same and only number parts may differ
                    sb.AppendLine(GetDuplicateLine(maxNumber, duplicateStringsBuffer[0]));
                }
                else if (ShouldBeDuplicate(duplicateStringDensity))
                {
                    sb.AppendLine(GetDuplicateLine(maxNumber, duplicateStringsBuffer[random.Next(0, duplicateStringsBuffer.Length - 1)]));
                }
                else
                {
                    sb.AppendLine(GetRandomLine(maxNumber, maxWordsCount, maxWordLength));
                }
            }

            // Optimizing to create a file with exactly (or at least the closest to) the required size - not significantly less or greater
            sb.Clear();

            int maxLineSize = GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, EstimatedSizeType.Max);
            int linesCountToFulfillExpectedSize = ushort.MaxValue / maxLineSize;

            for (int i = 0; i < linesCountToFulfillExpectedSize; i++)
            {
                if (cts.IsCancellationRequested)
                {
                    cts.ThrowIfCancellationRequested();
                }

                generatedLinesCount++;

                if (duplicateStringDensity >= Hundred)
                {
                    // All strings should be the same and only number parts may differ
                    sb.AppendLine(GetDuplicateLine(maxNumber, duplicateStringsBuffer[0]));
                }
                else if (ShouldBeDuplicate(duplicateStringDensity))
                {
                    sb.AppendLine(GetDuplicateLine(maxNumber, duplicateStringsBuffer[random.Next(0, duplicateStringsBuffer.Length - 1)]));
                }
                else
                {
                    sb.AppendLine(GetRandomLine(maxNumber, maxWordsCount, maxWordLength));
                }
            }

            File.AppendAllText(fileName, sb.ToString());

            return generatedLinesCount;
        }

        /// <summary>
        /// Estimates the maximum number of bytes required to represent a single line, based on the specified parameters and size estimation type.
        /// </summary>
        /// <remarks>The estimation assumes UTF-8 encoding and accounts for the number portion, word count, word length, and necessary spacing. The returned value is suitable for buffer allocation or sizing scenarios where an upper bound or typical size is needed.</remarks>
        /// <param name="maxNumber">The largest number that may appear at the start of the line. Determines the maximum digit count for the number portion.</param>
        /// <param name="maxWordsCount">The maximum number of words that may appear in the line.</param>
        /// <param name="maxWordLength">The maximum length, in characters, of any single word in the line.</param>
        /// <param name="estimatedSizeType">The type of size estimation to use. Specifies whether to estimate the minimum, average, or maximum possible line size.</param>
        /// <returns>The estimated size, in bytes, required to represent a line with the given constraints. The value depends on the specified estimation type.</returns>
        public static int GetEstimatedLineSizeInBytes(int maxNumber, int maxWordsCount, int maxWordLength, EstimatedSizeType estimatedSizeType)
        {
            // In the case of a randomly generated 1-digit number, a one-letter single word,
            // the line size would be exactly 4 bytes for UTF-8 - e.g., '3. A'
            const int MinLineSizeInBytes = 4;

            // The greatest possible line size would be:
            // Number part digits number + Max Words Count * Max Word Length + Max Words Count (for leading spaces)
            int maxLineSizeInBytes = maxNumber.ToString().Length + ((maxWordsCount + 1) * maxWordLength) + 1;

            switch (estimatedSizeType)
            {
                case EstimatedSizeType.Min:
                    return MinLineSizeInBytes;

                case EstimatedSizeType.Avg:
                    return (MinLineSizeInBytes + maxLineSizeInBytes) / 2;

                default:
                    return maxLineSizeInBytes;
            }
        }

        /// <summary>
        /// Determines whether a value should be considered a duplicate based on the specified density threshold.
        /// </summary>
        /// <param name="duplicateStringDensity">The threshold value, as a percentage from 1 to 100, representing the likelihood that a value is treated as a duplicate. Must be greater than 0.</param>
        /// <returns>true if the value should be considered a duplicate based on the specified density; otherwise, false.</returns>
        private static bool ShouldBeDuplicate(short duplicateStringDensity)
        {
            return duplicateStringDensity > decimal.Zero && random.Next(One, Hundred) <= duplicateStringDensity;
        }

        /// <summary>
        /// Generates a random line of text composed of a random number, a delimiter, and a sequence of random words.
        /// </summary>
        /// <param name="maxNumber">The exclusive upper bound for the random number to include in the line. Must be greater than zero.</param>
        /// <param name="maxWordsCount">The maximum number of words to generate in the sequence. Must be greater than zero.</param>
        /// <param name="maxWordLength">The maximum length of each generated word. Must be greater than zero.</param>
        /// <returns>A string containing a random number, a delimiter, and a sequence of random words formatted as a single line.</returns>
        private static string GetRandomLine(int maxNumber, int maxWordsCount, int maxWordLength)
        {
            return string.Format(AlphanumericEntity.LinePattern,
                GetRandomNumber(maxNumber),
                AlphanumericEntity.Delimiter,
                GetRandomString(maxWordsCount, maxWordLength));
        }

        /// <summary>
        /// Formats a line using a random number, a delimiter, and the specified duplicate string according to the alphanumeric entity pattern.
        /// </summary>
        /// <param name="maxNumber">The exclusive upper bound for the random number to include in the formatted line. Must be greater than zero.</param>
        /// <param name="duplicateString">The string to include as the duplicate portion in the formatted line.</param>
        /// <returns>A string formatted with a random number, a delimiter, and the specified duplicate string, following the alphanumeric entity line pattern.</returns>
        private static string GetDuplicateLine(int maxNumber, string duplicateString)
        {
            return string.Format(AlphanumericEntity.LinePattern,
                GetRandomNumber(maxNumber),
                AlphanumericEntity.Delimiter,
                duplicateString);
        }

        /// <summary>
        /// Generates a random integer that is greater than or equal to a predefined minimum and less than the specified maximum value.
        /// </summary>
        /// <param name="maxNumber">The exclusive upper bound for the random number to generate. Must be greater than the predefined minimum value.</param>
        /// <returns>A random integer greater than or equal to the predefined minimum value and less than <paramref name="maxNumber"/>.</returns>
        private static int GetRandomNumber(int maxNumber)
        {
            return random.Next(One, maxNumber);
        }
        
        /// <summary>
        /// Generates a random string consisting of multiple words, each composed of randomly selected characters.
        /// </summary>
        /// <param name="maxWordsCount">The maximum number of words to include in the generated string. Must be greater than 1.</param>
        /// <param name="maxWordLength">The maximum length, in characters, of each word in the generated string. Must be greater than 1.</param>
        /// <returns>A randomly generated string containing up to the specified number of words, with each word containing up to the specified number of characters. The first character of the string is capitalized.</returns>
        private static string GetRandomString(int maxWordsCount, int maxWordLength)
        {
            string retVal = string.Empty;

            int wordsCount = random.Next(One, maxWordsCount);
            int wordLength;
            for (int i = 0; i < wordsCount; i++)
            {
                wordLength = random.Next(1, maxWordLength);
                for (int j = 0; j < wordLength; j++)
                {
                    retVal += new string(Enumerable.Repeat(AllowedChars, wordLength).Select(s => s[random.Next(s.Length)]).ToArray()) + Space;
                }
            }

            return $"{retVal[0].ToString().ToUpper()}{retVal[One..]}".TrimEnd();
        }
    }
}
