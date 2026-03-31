using System.Diagnostics;

namespace MultiSorterLib.Tests
{
    [TestFixture]
    [Category("Stress")]
    public class StressTests
    {
        [Test]
        public void Stress_AlphanumericEntity_SortLargeArray_CompletesAndIsSorted()
        {
            const int count = 100_000;
            var rng = new Random(42);
            var entities = Enumerable.Range(0, count)
                .Select(_ => new AlphanumericEntity(
                    new string((char)('A' + rng.Next(26)), 1) + new string((char)('a' + rng.Next(26)), 1),
                    rng.Next(1, 100_000)))
                .ToArray();

            var sw = Stopwatch.StartNew();
            var sorted = entities.Order().ToArray();
            sw.Stop();

            Assert.That(sorted.Length, Is.EqualTo(count));
            for (int i = 1; i < sorted.Length; i++)
            {
                Assert.That(sorted[i - 1].CompareTo(sorted[i]), Is.LessThanOrEqualTo(0),
                    $"Sort order violated at index {i}");
            }
            Assert.That(sw.Elapsed.TotalSeconds, Is.LessThan(30), "Sort took too long.");
        }

        [Test]
        public void Stress_AlphanumericSorterHelper_LargeFile_SortsCorrectly()
        {
            const int lineCount = 50_000;
            string input = Path.Combine(Path.GetTempPath(), $"stress_sort_{Guid.NewGuid():N}.txt");
            string? outputPath = null;

            var rng = new Random(123);
            var lines = Enumerable.Range(0, lineCount)
                .Select(_ => $"{rng.Next(1, 99999)}. {(char)('A' + rng.Next(26))}{(char)('a' + rng.Next(26))}{(char)('a' + rng.Next(26))}")
                .ToArray();
            File.WriteAllLines(input, lines);

            try
            {
                var sw = Stopwatch.StartNew();
                using (var helper = new AlphanumericSorterHelper(input))
                {
                    outputPath = helper.OutputFileName;
                    helper.Sort();
                    helper.SaveOutputFile();
                }
                sw.Stop();

                Assert.That(File.Exists(outputPath), Is.True);
                var result = File.ReadAllLines(outputPath);
                Assert.That(result.Length, Is.EqualTo(lineCount));

                // Parse output back to entities and verify entity sort order
                var entities = AlphanumericSorterHelper.EnumerateEntities(result).ToArray();
                for (int i = 1; i < entities.Length; i++)
                {
                    Assert.That(entities[i - 1].CompareTo(entities[i]), Is.LessThanOrEqualTo(0),
                        $"Sort order violated at line {i}: '{result[i - 1]}' vs '{result[i]}'");
                }

                Assert.That(sw.Elapsed.TotalSeconds, Is.LessThan(60), "Large file sort took too long.");
            }
            finally
            {
                if (File.Exists(input)) File.Delete(input);
                if (outputPath != null && File.Exists(outputPath)) File.Delete(outputPath);
            }
        }

        [Test]
        public void Stress_ExternalSortAndMerge_EndToEnd()
        {
            const int lineCount = 20_000;
            string input = Path.Combine(Path.GetTempPath(), $"stress_ext_{Guid.NewGuid():N}.txt");
            string output = Path.Combine(Path.GetTempPath(), $"stress_ext_out_{Guid.NewGuid():N}.txt");
            var tempFiles = new List<string>();

            var rng = new Random(999);
            var lines = Enumerable.Range(0, lineCount)
                .Select(_ => $"{rng.Next(1, 99999)}. {(char)('A' + rng.Next(26))}{(char)('a' + rng.Next(26))}")
                .ToArray();
            File.WriteAllLines(input, lines);

            try
            {
                var sw = Stopwatch.StartNew();
                tempFiles = ExternalSorter.CreateSortedChunks(input);
                Assert.That(tempFiles.Count, Is.GreaterThanOrEqualTo(1));

                Merger.MergeSortedFiles(tempFiles, output);
                sw.Stop();

                Assert.That(File.Exists(output), Is.True);
                var result = File.ReadAllLines(output);
                Assert.That(result.Length, Is.GreaterThan(0));

                // Parse and verify entity sort order in merged output
                var entities = AlphanumericSorterHelper.EnumerateEntities(result).ToArray();
                for (int i = 1; i < entities.Length; i++)
                {
                    Assert.That(entities[i - 1].CompareTo(entities[i]), Is.LessThanOrEqualTo(0),
                        $"Merge sort order violated at line {i}");
                }

                Assert.That(sw.Elapsed.TotalSeconds, Is.LessThan(60), "External sort+merge took too long.");
            }
            finally
            {
                if (File.Exists(input)) File.Delete(input);
                if (File.Exists(output)) File.Delete(output);
                foreach (var f in tempFiles) if (File.Exists(f)) File.Delete(f);
            }
        }

        [Test]
        public void Stress_ManyDuplicateEntities_SortByNumeric()
        {
            const int count = 50_000;
            // All same string, different numbers -> stress the numeric comparison path
            var entities = Enumerable.Range(0, count)
                .Select(i => new AlphanumericEntity("Same", count - i))
                .ToArray();

            var sorted = entities.Order().ToArray();
            Assert.That(sorted.Length, Is.EqualTo(count));
            for (int i = 0; i < sorted.Length; i++)
            {
                Assert.That(sorted[i].NumericPart, Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void Stress_ConcurrentSorterInstances_NoInterference()
        {
            const int instanceCount = 10;
            const int linesPerFile = 1_000;
            var inputs = new string[instanceCount];
            var outputs = new string[instanceCount];

            try
            {
                // Create input files sequentially (Random is not thread-safe)
                for (int i = 0; i < instanceCount; i++)
                {
                    var rng = new Random(i * 1000);
                    inputs[i] = Path.Combine(Path.GetTempPath(), $"conc_{i}_{Guid.NewGuid():N}.txt");
                    var lines = Enumerable.Range(0, linesPerFile)
                        .Select(_ => $"{rng.Next(1, 9999)}. {(char)('A' + rng.Next(26))}{(char)('a' + rng.Next(26))}")
                        .ToArray();
                    File.WriteAllLines(inputs[i], lines);
                }

                // Sort concurrently
                Parallel.For(0, instanceCount, i =>
                {
                    using var helper = new AlphanumericSorterHelper(inputs[i]);
                    outputs[i] = helper.OutputFileName;
                    helper.Sort();
                    helper.SaveOutputFile();
                });

                // Verify each output is individually sorted
                for (int i = 0; i < instanceCount; i++)
                {
                    Assert.That(File.Exists(outputs[i]), Is.True, $"Output file {i} missing.");
                    var result = File.ReadAllLines(outputs[i]);
                    Assert.That(result.Length, Is.EqualTo(linesPerFile));

                    var entities = AlphanumericSorterHelper.EnumerateEntities(result).ToArray();
                    for (int j = 1; j < entities.Length; j++)
                    {
                        Assert.That(entities[j - 1].CompareTo(entities[j]), Is.LessThanOrEqualTo(0),
                            $"Instance {i} sort violated at line {j}");
                    }
                }
            }
            finally
            {
                foreach (var f in inputs) if (f != null && File.Exists(f)) File.Delete(f);
                foreach (var f in outputs) if (f != null && File.Exists(f)) File.Delete(f);
            }
        }

        [Test]
        public void Stress_FileSizeExtensions_ManyConversions_Consistent()
        {
            // Round-trip: value → bytes → value. Use Kilo/Mega/Giga where the multiplier
            // is large enough that 0.5-step values still produce whole-byte counts.
            // None (= raw bytes) is tested separately with integer values only,
            // because fractional bytes are truncated by the ulong cast.
            var scaledPrefixes = new[]
            {
                FileSizeExtensions.MetricPrefixes.Kilo,
                FileSizeExtensions.MetricPrefixes.Mega,
                FileSizeExtensions.MetricPrefixes.Giga,
            };

            for (int iteration = 0; iteration < 10_000; iteration++)
            {
                double original = 1.0 + (iteration % 100) * 0.5;

                foreach (var prefix in scaledPrefixes)
                {
                    ulong bytes = FileSizeExtensions.GetFileSizeInBytes(original, prefix);
                    double roundTrip = FileSizeExtensions.GetFileSizeInUnits(bytes, prefix);
                    Assert.That(roundTrip, Is.EqualTo(original).Within(0.01),
                        $"Round-trip failed for {original} {prefix} at iteration {iteration}");
                }

                // None prefix: use only whole-number values (fractional bytes don't exist)
                double wholeOriginal = Math.Floor(original);
                ulong noneBytes = FileSizeExtensions.GetFileSizeInBytes(wholeOriginal, FileSizeExtensions.MetricPrefixes.None);
                double noneRoundTrip = FileSizeExtensions.GetFileSizeInUnits(noneBytes, FileSizeExtensions.MetricPrefixes.None);
                Assert.That(noneRoundTrip, Is.EqualTo(wholeOriginal).Within(0.01),
                    $"Round-trip (None) failed for {wholeOriginal} at iteration {iteration}");
            }
        }

        [Test]
        [Category("Stress")]
        public void Stress_SortAllDifferentStrings_SameNumeric_OrdersByStringOnly()
        {
            const int count = 50_000;
            var rng = new Random(314);
            var entities = Enumerable.Range(0, count)
                .Select(_ => new AlphanumericEntity(
                    new string(Enumerable.Range(0, 4).Select(_ => (char)('A' + rng.Next(26))).ToArray()),
                    42))
                .ToArray();

            var sorted = entities.Order().ToArray();
            for (int i = 1; i < sorted.Length; i++)
            {
                Assert.That(
                    string.Compare(sorted[i - 1].StringPart, sorted[i].StringPart, StringComparison.Ordinal),
                    Is.LessThanOrEqualTo(0),
                    $"String sort violated at index {i}: '{sorted[i - 1].StringPart}' vs '{sorted[i].StringPart}'");
            }
        }

        [Test]
        [Category("Stress")]
        public void Stress_PairwiseComparison_Consistency()
        {
            const int pairCount = 100_000;
            var rng = new Random(555);
            for (int i = 0; i < pairCount; i++)
            {
                var a = new AlphanumericEntity(
                    new string((char)('A' + rng.Next(10)), 1), rng.Next(-1000, 1000));
                var b = new AlphanumericEntity(
                    new string((char)('A' + rng.Next(10)), 1), rng.Next(-1000, 1000));

                int ab = a.CompareTo(b);
                int ba = b.CompareTo(a);

                Assert.That(Math.Sign(ab), Is.EqualTo(-Math.Sign(ba)),
                    $"Antisymmetry violated for ({a.StringPart},{a.NumericPart}) vs ({b.StringPart},{b.NumericPart})");

                if (a.StringPart == b.StringPart && a.NumericPart == b.NumericPart)
                {
                    Assert.That(ab, Is.EqualTo(0));
                    Assert.That(a.Equals(b), Is.True);
                }
            }
        }

        [Test]
        [Category("Stress")]
        public void Stress_GetHashCode_EqualEntities_AlwaysSameHash()
        {
            const int count = 50_000;
            var rng = new Random(777);
            for (int i = 0; i < count; i++)
            {
                string s = new string((char)('A' + rng.Next(26)), rng.Next(1, 5));
                int n = rng.Next(-10_000, 10_000);
                var a = new AlphanumericEntity(s, n);
                var b = new AlphanumericEntity(s, n);

                Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()),
                    $"Hash mismatch for (\"{s}\", {n})");
            }
        }

        [Test]
        [Category("Stress")]
        public void Stress_GetHashCode_CollisionResistance()
        {
            const int count = 10_000;
            var hashes = new HashSet<int>();
            for (int i = 0; i < count; i++)
            {
                var e = new AlphanumericEntity($"Str{i}", i);
                hashes.Add(e.GetHashCode());
            }

            double uniqueRatio = (double)hashes.Count / count;
            Assert.That(uniqueRatio, Is.GreaterThan(0.90),
                $"Too many collisions: {hashes.Count}/{count} unique hashes ({uniqueRatio:P1})");
        }

        [Test]
        [Category("Stress")]
        public void Stress_EntityLine_MassGeneration_AllMatchExpectedFormat()
        {
            const int count = 100_000;
            var rng = new Random(888);
            for (int i = 0; i < count; i++)
            {
                string s = new string((char)('A' + rng.Next(26)), rng.Next(1, 6));
                int n = rng.Next(int.MinValue, int.MaxValue);
                var e = new AlphanumericEntity(s, n);

                string expected = $"{n}{AlphanumericEntity.Delimiter} {s}";
                Assert.That(e.EntityLine, Is.EqualTo(expected),
                    $"EntityLine mismatch at iteration {i}");
            }
        }

        [Test]
        [Category("Stress")]
        public void Stress_Sort_MixedNegativePositiveZero_CorrectOrder()
        {
            const int count = 20_000;
            var rng = new Random(202);
            var entities = Enumerable.Range(0, count)
                .Select(_ => new AlphanumericEntity("Mixed", rng.Next(int.MinValue, int.MaxValue)))
                .ToArray();

            var sorted = entities.Order().ToArray();
            for (int i = 1; i < sorted.Length; i++)
            {
                Assert.That(sorted[i - 1].NumericPart, Is.LessThanOrEqualTo(sorted[i].NumericPart),
                    $"Numeric order violated at index {i}: {sorted[i - 1].NumericPart} vs {sorted[i].NumericPart}");
            }
        }
    }
}
