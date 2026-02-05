namespace MultiSorterLib
{
    public sealed class AlphanumericSorterHelper : IDisposable
    {
        private readonly string? inputFileName;
        private IEnumerable<AlphanumericEntity>? entities;

        public AlphanumericSorterHelper(string fileName)
        {
            if (File.Exists(fileName))
            {
                inputFileName = fileName;

                entities = EnumerateEntities(File.ReadLines(inputFileName));
            }
        }
        
        public void Sort()
        {
            var sortedEntities = entities?.ToArray().Order();

            entities?.GetEnumerator().Dispose();
            entities = null;

            entities = sortedEntities;
        }

        public void SaveToFile(string fileName)
        {
            if (entities?.LongCount() > 0)
            {
                var directory = Path.GetDirectoryName(fileName);

                if (string.IsNullOrWhiteSpace(directory))
                {
                    directory = Path.GetDirectoryName(inputFileName);
                }

                if (!Directory.Exists(directory))
                {
                    // Suppressed the possible null reference warning, as the directory is known to be valid here
                    Directory.CreateDirectory(directory!);
                }

                File.WriteAllLines(fileName, entities.Select(s => s.EntityLine));
            }
        }
        
        public void Dispose()
        {
            entities?.GetEnumerator().Dispose();
            entities = null;
        }

        private static IEnumerable<string> GetSplit(string s, char c)
        {
            int l = s.Length;
            int i = 0, j = s.IndexOf(c, 0, l);

            if (j == -1) // No such substring
            {
                yield return s; // Return original and break
                yield break;
            }

            while (j != -1)
            {
                if (j - i > 0) // Non empty? 
                {
                    yield return s.Substring(i, j - i); // Return non-empty match
                }
                i = j + 1;
                j = s.IndexOf(c, i, l - i);
            }

            if (i < l) // Has remainder?
            {
                yield return s.Substring(i, l - i); // Return remaining trail
            }
        }

        private static IEnumerable<AlphanumericEntity> EnumerateEntities(IEnumerable<string> lines)
        {
            IEnumerator<string> enumerator = lines.GetEnumerator();

            try
            {
                string[]? parts;
                while (enumerator.MoveNext())
                {
                    parts = GetSplit(enumerator.Current, AlphanumericEntity.Delimiter).ToArray();

                    if (parts != null && parts.Length == 2 && int.TryParse(parts[0].Trim(), out int numericPart))
                    {
                        yield return new AlphanumericEntity(parts[1].Trim(), numericPart);
                    }
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
