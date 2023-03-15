namespace TestFileGenerator
{
    public static class RandomLineGenerator
    {
        private const int One = 1;
        private const string Space = " ";
        private const string Chars = "abcdefghijklmnopqrstuvwxyz";

        private static readonly Random random = new();

        public static int GetRandomNumber(int maxNumber = int.MaxValue)
        {
            return random.Next(One, maxNumber);
        }

        // TODO: Optimize and add string duplicate guarantee mechanism
        public static string GetRandomString(int maxWordsCount, int maxWordLength)
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
