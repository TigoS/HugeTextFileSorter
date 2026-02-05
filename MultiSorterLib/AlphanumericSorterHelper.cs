namespace MultiSorterLib
{
    /// <summary>
    /// Provides functionality to sort alphanumeric entities from an input file and save the sorted results to an output
    /// file.
    /// </summary>
    /// <remarks>This class reads entities from a specified input file, sorts them in ascending order, and
    /// writes the sorted entities to an output file with "_Sorted" appended to the original file name. The class
    /// manages resources associated with file operations and should be disposed when no longer needed. Instances are
    /// not thread-safe.</remarks>
    public sealed class AlphanumericSorterHelper : IDisposable
    {
        private readonly string? inputFileName;
        private readonly string? outputFileName;
        private IEnumerable<AlphanumericEntity>? entities;

        public string OutputFileName => outputFileName ?? string.Empty;

        /// <summary>
        /// Initializes a new instance of the AlphanumericSorterHelper class using the specified input file.
        /// </summary>
        /// <remarks>The output file will be created in the same directory as the input file, with
        /// "_Sorted" appended to its name before the file extension.</remarks>
        /// <param name="fileName">The path to the input file containing entities to be sorted. Must refer to an existing file.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="fileName"/> does not exist.</exception>
        public AlphanumericSorterHelper(string fileName)
        {
            if (File.Exists(fileName))
            {
                inputFileName = fileName;

                // The following `Path.GetDirectoryName()` possible nullable warning is suppressed
                //  as it's checked to be a valid directory path just a line above
                outputFileName = Path.Combine(
                    Path.GetDirectoryName(fileName)!,
                    Path.GetFileNameWithoutExtension(fileName) +
                    "_Sorted" + Path.GetExtension(fileName));

                entities = EnumerateEntities(File.ReadLines(inputFileName));
            }
            else
            {
                throw new FileNotFoundException("The specified input file was not found.", fileName);
            }
        }
        
        /// <summary>
        /// Sorts the collection of entities in ascending order.
        /// </summary>
        /// <remarks>After sorting, the original collection is replaced with the sorted sequence. If the
        /// collection is null, no action is taken.</remarks>
        public void Sort()
        {
            var sortedEntities = entities?.ToArray().Order();

            entities?.GetEnumerator().Dispose();
            entities = null;

            entities = sortedEntities;
        }

        /// <summary>
        /// Saves the output file containing all entity lines if an output file name is specified and entities are
        /// available.
        /// </summary>
        /// <remarks>This method writes each entity's line to the specified output file. The file is only
        /// created or overwritten if the output file name is not null, empty, or whitespace, and there is at least one
        /// entity present. If these conditions are not met, no file is written.</remarks>
        public void SaveOutputFile()
        {
            if (!string.IsNullOrWhiteSpace(outputFileName) && entities?.LongCount() > 0)
            {
                File.WriteAllLines(outputFileName, entities.Select(s => s.EntityLine));
            }
        }
        
        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        /// <remarks>Call this method when the instance is no longer needed to free associated resources.
        /// After calling Dispose, the instance should not be used.</remarks>
        public void Dispose()
        {
            entities?.GetEnumerator().Dispose();
            entities = null;
        }

        /// <summary>
        /// Splits the specified string into substrings based on the provided delimiter character.
        /// </summary>
        /// <remarks>Empty substrings are not included in the result. The method does not return empty
        /// entries between consecutive delimiters or at the start/end of the string.</remarks>
        /// <param name="s">The string to be split. If the delimiter is not found, the original string is returned as a single element.</param>
        /// <param name="c">The character used as the delimiter for splitting the string.</param>
        /// <returns>An enumerable collection of substrings resulting from splitting the input string by the specified delimiter.
        /// Substrings are non-empty, and the original string is returned as a single element if the delimiter is not
        /// present.</returns>
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

        /// <summary>
        /// Enumerates alphanumeric entities parsed from a sequence of input lines.
        /// </summary>
        /// <remarks>Lines that do not contain exactly two parts separated by the delimiter, or whose
        /// first part cannot be parsed as an integer, are ignored.</remarks>
        /// <param name="lines">The collection of strings to parse, where each line is expected to contain a numeric value and an
        /// alphanumeric identifier separated by the specified delimiter.</param>
        /// <returns>An enumerable collection of <see cref="AlphanumericEntity"/> objects parsed from the input lines. Only lines
        /// that can be successfully parsed into an alphanumeric entity are included.</returns>
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
