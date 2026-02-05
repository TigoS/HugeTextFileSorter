namespace MultiSorterLib
{
    public static class ExternalSorter
    {
        const int CHUNK_SIZE_MB = 256;
        const long CHUNK_SIZE_MULTIPLIER = 1024L;

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
