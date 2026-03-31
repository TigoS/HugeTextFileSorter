namespace MultiSorterLib
{
    /// <summary>
    /// Provides methods for merging multiple sorted text files into a single sorted output file.
    /// </summary>
    /// <remarks>The Merger class is intended for scenarios where large, pre-sorted text files need to be combined efficiently while preserving sort order. All methods are static and thread-safe when used with distinct file sets.</remarks>
    public static class Merger
    {
        /// <summary>
        /// Merges multiple sorted text files into a single sorted output file.
        /// </summary>
        /// <remarks>Each input file is expected to contain lines sorted in ascending order. The method reads all input files line by line and writes the merged, sorted result to the specified output file. Blank or whitespace-only lines are ignored. The method does not perform any validation on the contents of the files beyond line sorting. This method is not thread-safe.</remarks>
        /// <param name="files">A list of file paths to the input text files. Each file must be sorted in ascending order. Cannot be null or contain null or empty entries.</param>
        /// <param name="outputFile">The file path where the merged, sorted output will be written. If the file exists, it will be overwritten.
        /// Cannot be null or empty.</param>
        public static void MergeSortedFiles(List<string> files, string outputFile)
        {
            var readers = new List<StreamReader>();
            var heap = new PriorityQueue<(string value, int fileIndex), string>();
            StreamReader reader;
            string line;
            for (int i = 0; i < files.Count; i++)
            {
                reader = new StreamReader(files[i]);
                readers.Add(reader);

                if (!reader.EndOfStream)
                {
                    line = reader.ReadLine() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        heap.Enqueue((line, i), line);
                    }
                }
            }

            string next;
            using var writer = new StreamWriter(outputFile);
            while (heap.Count > 0)
            {
                var (value, fileIndex) = heap.Dequeue();
                writer.WriteLine(value);

                if (!readers[fileIndex].EndOfStream)
                {
                    next = readers[fileIndex].ReadLine() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(next))
                    {
                        heap.Enqueue((next, fileIndex), next);
                    }
                }
            }

            foreach (var r in readers)
            {
                r.Dispose();
            }
        }
    }
}
