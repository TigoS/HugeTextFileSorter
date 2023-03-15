using System.Text;
using static MultiSorterLib.FileSizeExtensions;

namespace MultiSorterLib
{
    public static class RandomFileGenerator
    {
        private const int One = 1;
        private const int Hundred = 100;
        private const string Space = " ";
        private const string Chars = "abcdefghijklmnopqrstuvwxyz";

        private static readonly Random random = new();
        
        public static long GenerateTestFile(
            string fileName,
            long fileSize,
            int maxNumber = int.MaxValue,
            int maxWordsCount = 150,
            int maxWordLength = 20,
            short duplicateStringDensity = 0)
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

            while (fileInfo.Length < fileSize - ushort.MaxValue)
            {
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

        private static bool ShouldBeDuplicate(short duplicateStringDensity)
        {
            return duplicateStringDensity > decimal.Zero && random.Next(One, Hundred) <= duplicateStringDensity;
        }

        private static string GetRandomLine(int maxNumber, int maxWordsCount, int maxWordLength)
        {
            return string.Format(AlphanumericEntity.LinePattern,
                GetRandomNumber(maxNumber),
                AlphanumericEntity.Delimiter,
                GetRandomString(maxWordsCount, maxWordLength));
        }

        private static string GetDuplicateLine(int maxNumber, string duplicateString)
        {
            return string.Format(AlphanumericEntity.LinePattern,
                GetRandomNumber(maxNumber),
                AlphanumericEntity.Delimiter,
                duplicateString);
        }

        private static int GetRandomNumber(int maxNumber)
        {
            return random.Next(One, maxNumber);
        }
        
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
                    retVal += new string(Enumerable.Repeat(Chars, wordLength).Select(s => s[random.Next(s.Length)]).ToArray()) + Space;
                }
            }

            return $"{retVal[0].ToString().ToUpper()}{retVal[One..]}".TrimEnd();
        }
    }
}
