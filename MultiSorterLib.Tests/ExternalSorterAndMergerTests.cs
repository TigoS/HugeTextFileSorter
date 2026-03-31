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
                    var entities = AlphanumericSorterHelper.EnumerateEntities(chunkLines);
                    var sortedEntities = entities.ToArray().Order();

                    entities.GetEnumerator().Dispose();
                    entities = null;

                    var sorted = sortedEntities.Select(s => s.EntityLine).ToArray();


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

        [Test]
        public void CreateSortedChunks_EmptyFile_ReturnsEmptyList()
        {
            string input = Path.Combine(Path.GetTempPath(), $"extsort_empty_{Guid.NewGuid():N}.txt");
            File.WriteAllText(input, string.Empty);
            try
            {
                var result = ExternalSorter.CreateSortedChunks(input);
                Assert.That(result, Is.Empty);
            }
            finally
            {
                File.Delete(input);
            }
        }

        [Test]
        public void CreateSortedChunks_OnlyWhitespaceLines_ProducesEmptyChunks()
        {
            string input = Path.Combine(Path.GetTempPath(), $"extsort_ws_{Guid.NewGuid():N}.txt");
            File.WriteAllLines(input, ["", "   ", "\t", "  "]);
            var tempFiles = new List<string>();
            try
            {
                tempFiles = ExternalSorter.CreateSortedChunks(input);
                foreach (var f in tempFiles)
                {
                    var lines = File.ReadAllLines(f);
                    Assert.That(lines, Is.Empty);
                }
            }
            finally
            {
                File.Delete(input);
                foreach (var f in tempFiles) if (File.Exists(f)) File.Delete(f);
            }
        }

        [Test]
        public void CreateSortedChunks_SingleValidLine_ReturnsSingleChunk()
        {
            string input = Path.Combine(Path.GetTempPath(), $"extsort_single_{Guid.NewGuid():N}.txt");
            File.WriteAllLines(input, ["42. Hello"]);
            var tempFiles = new List<string>();
            try
            {
                tempFiles = ExternalSorter.CreateSortedChunks(input);
                Assert.That(tempFiles, Has.Count.EqualTo(1));
                var lines = File.ReadAllLines(tempFiles[0]);
                Assert.That(lines, Has.Length.EqualTo(1));
                Assert.That(lines[0], Does.Contain("Hello"));
            }
            finally
            {
                File.Delete(input);
                foreach (var f in tempFiles) if (File.Exists(f)) File.Delete(f);
            }
        }

        [Test]
        public void MergeSortedFiles_SingleFile_ProducesIdenticalOutput()
        {
            string temp = Path.GetTempFileName();
            var content = new[] { "1. Alpha", "2. Beta", "3. Gamma" };
            File.WriteAllLines(temp, content);
            string output = Path.Combine(Path.GetTempPath(), $"merge_single_{Guid.NewGuid():N}.txt");
            try
            {
                Merger.MergeSortedFiles(new List<string> { temp }, output);
                var lines = File.ReadAllLines(output);
                Assert.That(lines, Is.EqualTo(content).AsCollection);
            }
            finally
            {
                File.Delete(temp);
                if (File.Exists(output)) File.Delete(output);
            }
        }

        [Test]
        public void MergeSortedFiles_EmptyFiles_ProducesEmptyOutput()
        {
            string temp1 = Path.GetTempFileName();
            string temp2 = Path.GetTempFileName();
            string output = Path.Combine(Path.GetTempPath(), $"merge_empty_{Guid.NewGuid():N}.txt");
            try
            {
                Merger.MergeSortedFiles(new List<string> { temp1, temp2 }, output);
                var lines = File.ReadAllLines(output);
                Assert.That(lines, Is.Empty);
            }
            finally
            {
                File.Delete(temp1);
                File.Delete(temp2);
                if (File.Exists(output)) File.Delete(output);
            }
        }

        [Test]
        public void MergeSortedFiles_ThreeFiles_MergesInSortedOrder()
        {
            string temp1 = Path.GetTempFileName();
            string temp2 = Path.GetTempFileName();
            string temp3 = Path.GetTempFileName();

            var a = new[] { "1. A", "4. D" };
            var b = new[] { "2. B", "5. E" };
            var c = new[] { "3. C", "6. F" };
            File.WriteAllLines(temp1, a);
            File.WriteAllLines(temp2, b);
            File.WriteAllLines(temp3, c);

            string output = Path.Combine(Path.GetTempPath(), $"merge_three_{Guid.NewGuid():N}.txt");
            try
            {
                Merger.MergeSortedFiles([temp1, temp2, temp3], output);
                var lines = File.ReadAllLines(output);
                var expected = a.Concat(b).Concat(c).OrderBy(s => s, StringComparer.Ordinal).ToArray();
                Assert.That(lines, Is.EqualTo(expected).AsCollection);
            }
            finally
            {
                File.Delete(temp1);
                File.Delete(temp2);
                File.Delete(temp3);
                if (File.Exists(output)) File.Delete(output);
            }
        }
    }
}