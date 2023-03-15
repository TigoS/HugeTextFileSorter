using System.Text;
using MultiSorterLib;

namespace TestFileGenerator
{
    // TODO: Consider moving to the MultiSorterLib DLL
    public static class RandomFileGenerator
    {
        private const int One = 1;
        private const string Space = " ";
        private const string Chars = "abcdefghijklmnopqrstuvwxyz";

        private static readonly Random random = new();
        
        public static long GenerateTestFile(
            string fileName,
            long fileSize,
            int maxNumber = int.MaxValue,
            int maxWordsCount = 100,
            int maxWordLength = 12)
        {
            long generatedLinesCount = 0;
            StringBuilder sb = new StringBuilder();
            FileInfo fileInfo = new FileInfo(fileName);

            while (fileInfo.Length < fileSize - ushort.MaxValue)
            {
                generatedLinesCount++;

                if (sb.Length >= ushort.MaxValue)
                {
                    File.AppendAllText(fileName, sb.ToString());
                    sb.Clear();

                    fileInfo = new FileInfo(fileName);
                }

                sb.AppendLine(string.Format(AlphanumericEntity.LinePattern,
                    GetRandomNumber(maxNumber),
                    AlphanumericEntity.Delimiter,
                    GetRandomString(maxWordsCount, maxWordLength)));
            }

            return generatedLinesCount;
        }

        private static int GetRandomNumber(int maxNumber = int.MaxValue)
        {
            return random.Next(One, maxNumber);
        }

        // TODO: Optimize and add string duplicate guarantee mechanism
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
