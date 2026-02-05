namespace MultiSorterLib
{
    public static class Merger
    {
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
