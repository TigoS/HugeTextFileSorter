namespace TestFileGenerator
{
    public static class RandomLineGenerator
    {
        private static readonly Random random = new();

        public static int GetRandomNumber(int maxNumber = int.MaxValue)
        {
            return random.Next(1, maxNumber);
        }
        
        public static string GetRandomString(int maxWordsCount, int maxWordLength)
        {
            string retVal = string.Empty;

            int wordsCount = random.Next(1, maxWordsCount);
            for (int i = 0; i < wordsCount; i++)
            {
                int wordLength = random.Next(1, maxWordLength);
                for (int j = 0; j < wordLength; j++)
                {
                    retVal += new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", wordLength).Select(s => s[random.Next(s.Length)]).ToArray()) + " ";
                }
            }

            return retVal.TrimEnd();
        }
    }
}
