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
            var sortedEntities = entities?.ToHashSet().Order();

            entities?.GetEnumerator().Dispose();
            entities = null;

            entities = sortedEntities;

            // TODO: Temporary hack to reduce memory consumption
            sortedEntities = null;
            sortedEntities?.GetEnumerator().Dispose();
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

        public void CleanupEntities()
        {
            entities?.GetEnumerator().Dispose();
            entities = null;
        }
        
        public void Dispose()
        {
            CleanupEntities();
        }

        private static IEnumerable<AlphanumericEntity> EnumerateEntities(IEnumerable<string> lines)
        {
            IEnumerator<string> enumerator = lines.GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    var parts = enumerator.Current.Split(AlphanumericEntity.Delimiter);

                    if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int numericPart))
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
