using MultiSorterLib;
using System.Reflection;

namespace TestFileGenerator.Tests
{
    [TestFixture]
    public class AlphanumericSorterHelperTests
    {
        private static MethodInfo GetPrivate(string name, params Type[] types) =>
            typeof(AlphanumericSorterHelper).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static, null, types, null)
            ?? throw new InvalidOperationException($"Method '{name}' not found.");

        private static MethodInfo GetPublic(string name, params Type[] types) =>
            typeof(AlphanumericSorterHelper).GetMethod(name, BindingFlags.Public | BindingFlags.Static, null, types, null)
            ?? throw new InvalidOperationException($"Method '{name}' not found.");

        [Test]
        public void Constructor_WhenInputFileMissing_ThrowsFileNotFoundException()
        {
            string missing = Path.Combine(Path.GetTempPath(), $"missing_{Guid.NewGuid():N}.txt");
            var ex = Assert.Throws<FileNotFoundException>(() => new AlphanumericSorterHelper(missing));
            Assert.That(ex!.FileName, Is.EqualTo(missing));
        }

        [Test]
        [Category("Integration")]
        public void SortAndSaveOutputFile_WritesSortedEntities_WithExpectedSuffix()
        {
            string dir = Path.GetTempPath();
            string input = Path.Combine(dir, $"input_{Guid.NewGuid():N}.txt");
            string[] lines =
            {
                "2. Beta",
                "1. Alpha",
                "3. Alpha",   // same string different number should be sorted with string then numeric
                "invalid line",
                "  10 .  Zeta  " // deliberate spaces; should parse: number=10, string="Zeta"
            };
            File.WriteAllLines(input, lines);

            string outputPath;
            try
            {
                using var helper = new AlphanumericSorterHelper(input);
                outputPath = helper.OutputFileName;

                helper.Sort();
                helper.SaveOutputFile();

                Assert.That(File.Exists(outputPath), Is.True);
                var result = File.ReadAllLines(outputPath);
                // Expected order by StringPart then NumericPart:
                // Alpha 1, Alpha 3, Beta 2, Zeta 10
                Assert.That($"{1}{AlphanumericEntity.Delimiter} Alpha", Is.EqualTo(result[0]));
                Assert.That($"{3}{AlphanumericEntity.Delimiter} Alpha", Is.EqualTo(result[1]));
                Assert.That($"{2}{AlphanumericEntity.Delimiter} Beta", Is.EqualTo(result[2]));
                Assert.That($"{10}{AlphanumericEntity.Delimiter} Zeta", Is.EqualTo(result[3]));
            }
            finally
            {
                if (File.Exists(input)) File.Delete(input);
                // Output file path derived by helper
                var outFile = Path.Combine(Path.GetDirectoryName(input)!, Path.GetFileNameWithoutExtension(input) + "_Sorted" + Path.GetExtension(input));
                if (File.Exists(outFile)) File.Delete(outFile);
            }
        }

        [Test]
        public void GetSplit_SplitsOnDelimiter_ExcludesEmptyEntries()
        {
            var mi = GetPrivate("GetSplit", typeof(string), typeof(char));
            var res = (IEnumerable<string>)mi.Invoke(null, ["A..B.C..", '.']);
            var arr = res.ToArray();
            Assert.That(arr, Is.EqualTo(new[] { "A", "B", "C" }).AsCollection);
        }

        [Test]
        public void EnumerateEntities_ParsesValidLines_IgnoresInvalid()
        {
            var mi = GetPublic("EnumerateEntities", typeof(IEnumerable<string>));
            var input = new[] { "1. A", "bad", "  2  . B  ", "C. 3", "3 . C" };
            var res = ((IEnumerable<AlphanumericEntity>)mi.Invoke(null, [input])).ToArray();

            Assert.That(res.Length, Is.EqualTo(3));
            Assert.That(res[0].StringPart, Is.EqualTo("A"));
            Assert.That(res[1].StringPart, Is.EqualTo("B"));
            Assert.That(res[2].StringPart, Is.EqualTo("C"));
            Assert.That(res[0].NumericPart, Is.EqualTo(1));
            Assert.That(res[1].NumericPart, Is.EqualTo(2));
            Assert.That(res[2].NumericPart, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_ValidFile_SetsOutputFileNameWithSortedSuffix()
        {
            string dir = Path.GetTempPath();
            string input = Path.Combine(dir, $"test_{Guid.NewGuid():N}.txt");
            File.WriteAllText(input, "1. A");
            try
            {
                using var helper = new AlphanumericSorterHelper(input);
                string expected = Path.Combine(dir,
                    Path.GetFileNameWithoutExtension(input) + "_Sorted" + Path.GetExtension(input));
                Assert.That(helper.OutputFileName, Is.EqualTo(expected));
            }
            finally
            {
                File.Delete(input);
            }
        }

        [Test]
        public void Sort_EmptyFile_DoesNotThrow()
        {
            string input = Path.Combine(Path.GetTempPath(), $"empty_{Guid.NewGuid():N}.txt");
            File.WriteAllText(input, string.Empty);
            try
            {
                using var helper = new AlphanumericSorterHelper(input);
                Assert.DoesNotThrow(() => helper.Sort());
            }
            finally
            {
                File.Delete(input);
            }
        }

        [Test]
        public void SaveOutputFile_NoValidEntities_DoesNotCreateFile()
        {
            string input = Path.Combine(Path.GetTempPath(), $"invalid_{Guid.NewGuid():N}.txt");
            File.WriteAllLines(input, ["no delimiter here", "also invalid"]);
            try
            {
                using var helper = new AlphanumericSorterHelper(input);
                helper.Sort();
                helper.SaveOutputFile();
                Assert.That(File.Exists(helper.OutputFileName), Is.False);
            }
            finally
            {
                File.Delete(input);
            }
        }

        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            string input = Path.Combine(Path.GetTempPath(), $"dispose_{Guid.NewGuid():N}.txt");
            File.WriteAllText(input, "1. Test");
            try
            {
                var helper = new AlphanumericSorterHelper(input);
                Assert.DoesNotThrow(() =>
                {
                    helper.Dispose();
                    helper.Dispose();
                });
            }
            finally
            {
                File.Delete(input);
            }
        }

        [Test]
        public void GetSplit_NoDelimiter_ReturnsOriginalString()
        {
            var mi = GetPrivate("GetSplit", typeof(string), typeof(char));
            var res = ((IEnumerable<string>)mi.Invoke(null, ["NoDelimiterHere", '.'])!).ToArray();
            Assert.That(res, Is.EqualTo(new[] { "NoDelimiterHere" }).AsCollection);
        }

        [Test]
        public void GetSplit_SingleCharDelimiter_ReturnsEmpty()
        {
            var mi = GetPrivate("GetSplit", typeof(string), typeof(char));
            var res = ((IEnumerable<string>)mi.Invoke(null, [".", '.'])!).ToArray();
            Assert.That(res, Is.Empty);
        }

        [Test]
        public void GetSplit_LeadingAndTrailingDelimiters_SkipsThem()
        {
            var mi = GetPrivate("GetSplit", typeof(string), typeof(char));
            var res = ((IEnumerable<string>)mi.Invoke(null, [".A.B.", '.'])!).ToArray();
            Assert.That(res, Is.EqualTo(new[] { "A", "B" }).AsCollection);
        }

        [Test]
        public void EnumerateEntities_EmptyInput_ReturnsEmpty()
        {
            var entities = AlphanumericSorterHelper.EnumerateEntities(Array.Empty<string>()).ToArray();
            Assert.That(entities, Is.Empty);
        }

        [Test]
        public void EnumerateEntities_AllInvalidLines_ReturnsEmpty()
        {
            var lines = new[] { "nope", "bad line", "123", ". missing number", "abc. def" };
            var entities = AlphanumericSorterHelper.EnumerateEntities(lines).ToArray();
            Assert.That(entities, Is.Empty);
        }

        [Test]
        public void EnumerateEntities_LineWithMultipleDelimiters_IsSkipped()
        {
            // "1. Alpha. Beta" -> GetSplit yields 3 parts -> Length != 2 -> skipped
            var lines = new[] { "1. Alpha. Beta" };
            var entities = AlphanumericSorterHelper.EnumerateEntities(lines).ToArray();
            Assert.That(entities, Is.Empty);
        }
    }
}