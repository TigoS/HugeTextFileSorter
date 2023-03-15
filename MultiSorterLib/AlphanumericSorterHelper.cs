namespace MultiSorterLib
{
    public sealed class AlphanumericSorterHelper
    {
        public AlphanumericSorterHelper()
        {
            Entities = Enumerable.Empty<AlphanumericEntity>();
        }

        public AlphanumericSorterHelper(IEnumerable<string> lines)
        {
            Entities = EnumerateEntities(lines);
        }

        public IEnumerable<AlphanumericEntity> Entities;

        public long EntitiesCount => Entities.LongCount();

        public static AlphanumericSorterHelper LoadFromFile(string fileName)
        {
            return File.Exists(fileName) ? new AlphanumericSorterHelper(File.ReadLines(fileName)) : new AlphanumericSorterHelper();
        }
        
        public void Sort()
        {
            var entityArray = Entities.ToArray();
            Array.Sort(entityArray);
        }

        public void SaveToFile(string fileName)
        {
            if (EntitiesCount > 0)
            {
                var directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllLines(fileName, Entities.Select(s => s.EntityLine));
            }
        }

        public void CleanupEntities()
        {
            Entities.GetEnumerator().Dispose();
            Entities = Enumerable.Empty<AlphanumericEntity>();
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
