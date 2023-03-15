namespace MultiSorterLib
{
    public sealed class AlphanumericSorterHelper : IDisposable
    {
        private IEnumerable<AlphanumericEntity>? entities;

        private AlphanumericSorterHelper()
        {
            entities = null;
        }

        private AlphanumericSorterHelper(IEnumerable<string> lines)
        {
            entities = EnumerateEntities(lines);
        }

        public static AlphanumericSorterHelper LoadFromFile(string fileName)
        {
            return File.Exists(fileName) ? new AlphanumericSorterHelper(File.ReadLines(fileName)) : new AlphanumericSorterHelper();
        }

        // Adding temporary switcher to compare sorting performances by Array vs HashSet
        public void Sort(bool isArray)
        {
            if (isArray)
            {
                var entityArray = entities?.ToArray();

                if (entityArray?.Length > 0)
                {
                    Array.Sort(entityArray);
                }
            }
            else
            {
                var sortedEntities = entities?.ToHashSet().Order();

                entities?.GetEnumerator().Dispose();
                entities = null;

                entities = sortedEntities;
            }
        }

        public void SaveToFile(string fileName)
        {
            if (entities?.LongCount() > 0)
            {
                var directory = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(directory))
                {
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
