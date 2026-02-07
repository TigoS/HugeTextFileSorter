using Moq;
using MultiSorterLib;
using System.Reflection;

namespace TestFileGenerator.Tests
{   
    [TestFixture]
    public class RandomFileGeneratorTests
    {
        private static MethodInfo GetPrivateMethod(string name) =>
            typeof(RandomFileGenerator).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Method '{name}' not found.");

        [Test]
        public void GetEstimatedLineSizeInBytes_MinAvgMax_ReturnsExpectedValues()
        {
            int maxNumber = 9;
            int maxWordsCount = 3;
            int maxWordLength = 4;

            int min = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, FileSizeExtensions.EstimatedSizeType.Min);
            int max = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, FileSizeExtensions.EstimatedSizeType.Max);
            int avg = RandomFileGenerator.GetEstimatedLineSizeInBytes(maxNumber, maxWordsCount, maxWordLength, FileSizeExtensions.EstimatedSizeType.Avg);

            const int expectedMin = 4;
            int expectedMax = maxNumber.ToString().Length + ((maxWordsCount + 1) * maxWordLength) + 1;
            int expectedAvg = (expectedMin + expectedMax) / 2;

            Assert.That(min, Is.EqualTo(expectedMin));
            Assert.That(max, Is.EqualTo(expectedMax));
            Assert.That(avg, Is.EqualTo(expectedAvg));
        }

        [Test]
        public void ShouldBeDuplicate_BoundaryValues_ZeroReturnsFalse_100ReturnsTrue()
        {
            MethodInfo shouldBeDuplicate = GetPrivateMethod("ShouldBeDuplicate");

            // 0 should always be false (density > 0 required)
            object resultZero = shouldBeDuplicate.Invoke(null, [(short)0]);
            Assert.That(resultZero as bool?, Is.False);

            // 100 should always return true because random.Next(1,100) yields values 1..99 which are <= 100
            object resultHundred = shouldBeDuplicate.Invoke(null, [(short)100]);
            Assert.That(resultHundred, Is.Not.Null);
            Assert.That((bool)resultHundred!, Is.True);
        }

        [Test]
        public void GetRandomNumber_WithinBounds_ForSmallMax_ReturnsExpected()
        {
            MethodInfo getRandomNumber = GetPrivateMethod("GetRandomNumber");

            // For maxNumber = 2, random.Next(1,2) should always return 1
            object result = getRandomNumber.Invoke(null, [2]);
            int number = (int)result;
            Assert.That(number, Is.EqualTo(1));

            // For a larger maxNumber ensure result is within [1, maxNumber-1] across multiple samples
            int maxNumber = 20;
            for (int i = 0; i < 50; i++)
            {
                int value = (int)getRandomNumber.Invoke(null, [maxNumber]);
                Assert.That(value, Is.InRange(1, maxNumber - 1));
            }
        }

        [Test]
        public void GetRandomString_GeneratesNonEmpty_CapitalizedAndContainsOnlyLettersAndSpaces()
        {
            MethodInfo getRandomString = GetPrivateMethod("GetRandomString");

            // Use small bounds to keep the generated string short
            string result = (string)getRandomString.Invoke(null, [3, 3]);

            Assert.That(string.IsNullOrWhiteSpace(result), Is.False);
            Assert.That(char.IsUpper(result[0]), Is.True);
            Assert.That(result.EndsWith(" "), Is.False, "Result should not have trailing space.");

            // Only letters and spaces expected
            foreach (char c in result)
            {
                Assert.That(char.IsLetter(c) || c == ' ', Is.True, $"Unexpected char '{c}' in generated string.");
            }
        }

        [Test]
        public void GetRandomLine_And_GetDuplicateLine_FormatCorrectly()
        {
            MethodInfo getRandomLine = GetPrivateMethod("GetRandomLine");
            MethodInfo getDuplicateLine = GetPrivateMethod("GetDuplicateLine");

            string randomLine = (string)getRandomLine.Invoke(null, [100, 5, 5]);
            Assert.That(string.IsNullOrWhiteSpace(randomLine), Is.False);
            // Should contain delimiter '.' and a space after delimiter as per pattern "{0}{1} {2}"
            Assert.That(randomLine, Is.Not.Null, "randomLine should not be null.");
            Assert.That(randomLine.Contains(MultiSorterLib.AlphanumericEntity.Delimiter.ToString()), Is.True);
            Assert.That(randomLine.Contains(" "), Is.True);

            string dupSeed = "Alpha Beta";
            string dupLine = (string)getDuplicateLine.Invoke(null, [100, dupSeed]);
            Assert.That(dupLine, Is.Not.Null, "dupLine should not be null.");
            Assert.That(dupLine.Contains(MultiSorterLib.AlphanumericEntity.Delimiter.ToString()), Is.True);
            Assert.That(dupLine.TrimEnd().EndsWith(dupSeed), Is.True);
        }

        [Test]
        public void GenerateTestFile_CreatesFileAndReturnsPositiveLineCount()
        {
            string file = Path.Combine(Path.GetTempPath(), $"rfg_test_{Guid.NewGuid():N}.txt");

            try
            {
                ulong fileSize = 200_000; // moderate size to allow generation
                long lines = RandomFileGenerator.GenerateTestFile(
                    file,
                    fileSize,
                    maxNumber: 100,
                    maxWordsCount: 5,
                    maxWordLength: 5,
                    duplicateStringDensity: 0,
                    cts: CancellationToken.None);

                Assert.That(File.Exists(file), Is.True, "Expected file to be created.");
                FileInfo fi = new FileInfo(file);
                Assert.That(fi.Length > 0, Is.True, "Generated file should have non-zero length.");
                Assert.That(lines, Is.GreaterThan(0), "Returned line count should be positive.");
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        [Test]
        public void GenerateTestFile_ThrowsOperationCanceledException_WhenTokenCanceledBeforeStart()
        {
            string file = Path.Combine(Path.GetTempPath(), $"rfg_cancel_test_{Guid.NewGuid():N}.txt");

            try
            {
                ulong fileSize = 1_000_000; // must be greater than ushort.MaxValue to enter generation loop
                using CancellationTokenSource cts = new CancellationTokenSource();
                cts.Cancel(); // cancel before call

                Assert.Throws<OperationCanceledException>(() =>
                    RandomFileGenerator.GenerateTestFile(
                        file,
                        fileSize,
                        maxNumber: 100,
                        maxWordsCount: 5,
                        maxWordLength: 5,
                        duplicateStringDensity: 0,
                        cts: cts.Token));
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        // Small test to ensure Moq is available and functioning in the test project.
        [Test]
        public void Moq_IsAvailable_VerifySimpleMockWorks()
        {
            var mock = new Mock<IDisposable>();
            mock.Object.Dispose();
            mock.Verify(m => m.Dispose(), Times.Once);
        }
    }
}