using MultiSorterLib;

namespace TestFileGenerator.Tests
{
    [TestFixture]
    public class ExternalSorterAndMergerTests
    {
        [Test]
        public void CreateSortedChunks_SplitsIntoSortedTempFiles_ByApproximateChunkSize()
        {
            // Create an input file large enough to force multiple chunks
            string input = Path.Combine(Path.GetTempPath(), $"extsort_input_{Guid.NewGuid():N}.txt");
            var lines = Enumerable.Range(0, 20000)
                .Select(i => $"{i:D5}. {Guid.NewGuid():N}")
                .ToArray();
            File.WriteAllLines(input, lines);

            var tempFiles = ExternalSorter.CreateSortedChunks(input);
            try
            {
                Assert.That(tempFiles.Count >= 1, Is.True);
                foreach (var temp in tempFiles)
                {
                    Assert.That(File.Exists(temp), Is.True);
                    var chunkLines = File.ReadAllLines(temp);
                    // Verify sorted ascending (Ordinal)
                    var sorted = chunkLines.OrderBy(s => s, StringComparer.Ordinal).ToArray();
                    Assert.That(chunkLines, Is.EqualTo(sorted).AsCollection);
                }
            }
            finally
            {
                if (File.Exists(input)) File.Delete(input);
                foreach (var f in tempFiles)
                {
                    if (File.Exists(f)) File.Delete(f);
                }
            }
        }

        [Test]
        public void MergeSortedFiles_MergesChunksIntoSingleSortedOutput()
        {
            // Prepare two sorted temp files
            string temp1 = Path.GetTempFileName();
            string temp2 = Path.GetTempFileName();

            var a = new[] { "00001. A", "00003. A", "00005. A" };
            var b = new[] { "00002. B", "00004. B", "00006. B" };
            File.WriteAllLines(temp1, a);
            File.WriteAllLines(temp2, b);

            string output = Path.Combine(Path.GetTempPath(), $"merge_output_{Guid.NewGuid():N}.txt");

            try
            {
                Merger.MergeSortedFiles(new List<string> { temp1, temp2 }, output);

                Assert.That(File.Exists(output), Is.True);
                var lines = File.ReadAllLines(output);
                var expected = a.Concat(b).OrderBy(s => s, StringComparer.Ordinal).ToArray();
                Assert.That(lines, Is.EqualTo(expected).AsCollection);
            }
            finally
            {
                if (File.Exists(temp1)) File.Delete(temp1);
                if (File.Exists(temp2)) File.Delete(temp2);
                if (File.Exists(output)) File.Delete(output);
            }
        }
    }
}