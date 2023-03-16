using BenchmarkDotNet.Attributes;
using MultiSorterLib;

namespace AlphanumericSorterBenchmark
{
    [MemoryDiagnoser()]
    public class BenchmarkSorter : IDisposable
    {
        private static readonly double TestFileRelativeSize = 100;
        private static readonly FileSizeExtensions.MetricPrefixes MetricPrefix = FileSizeExtensions.MetricPrefixes.Giga;

        private readonly long InputFileSize;
        private readonly string InputFileName;
        private readonly string OutputFileName;

        private long generatedLinesCount;
        private AlphanumericSorterHelper? sorterHelper;

        public BenchmarkSorter()
        {
            var storageDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "AltiumTestProject",
                "TempStorage");

            if (!Directory.Exists(storageDirectory))
            {
                Directory.CreateDirectory(storageDirectory);
            }

            InputFileSize = TestFileRelativeSize.GetFileSizeInBytes(MetricPrefix);
            var formattedFileSize = ((double)InputFileSize).FormatFileSize();

            InputFileName = Path.Combine(storageDirectory, $"{formattedFileSize}_Benchmark_Test.txt");
            OutputFileName = Path.Combine(storageDirectory, $"{formattedFileSize}_Benchmark_Test_SORTED.txt");

            // Generate benchmark test file
            GenerateTestFile();

            // Sorting
            InitializeSorter();
            Sort();
            SaveToFile();
        }

        [Benchmark]
        public void GenerateTestFile()
        {
            generatedLinesCount = RandomFileGenerator.GenerateTestFile(InputFileName, InputFileSize);
        }

        [Benchmark]
        public void InitializeSorter()
        {
            if (generatedLinesCount > 0)
            {
                sorterHelper = new AlphanumericSorterHelper(InputFileName);
            }
        }

        [Benchmark]
        public void Sort()
        {
            sorterHelper?.Sort();
        }

        [Benchmark]
        public void SaveToFile()
        {
            sorterHelper?.SaveToFile(OutputFileName);
        }

        public void Dispose()
        {
            sorterHelper?.Dispose();
            sorterHelper = null;

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
