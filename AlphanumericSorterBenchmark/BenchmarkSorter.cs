using BenchmarkDotNet.Attributes;
using MultiSorterLib;
using TestFileGenerator;

namespace AlphanumericSorterBenchmark
{
    /// <summary>
    /// Provides benchmark methods for generating, sorting, and saving large alphanumeric test files to evaluate sorting
    /// performance.
    /// </summary>
    /// <remarks>This class is intended for use with benchmarking frameworks such as BenchmarkDotNet. It
    /// automates the setup, execution, and cleanup of sorting benchmarks, including test file generation and resource
    /// management. The class is not thread-safe.</remarks>
    [MemoryDiagnoser()]
    public class BenchmarkSorter : IDisposable
    {
        private static readonly double TestFileRelativeSize = 100;
        private static readonly FileSizeExtensions.MetricPrefixes MetricPrefix = FileSizeExtensions.MetricPrefixes.Mega;

        private readonly long inputFileSize;
        private readonly string inputFileName;

        private string outputFileName = string.Empty;
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

            inputFileSize = TestFileRelativeSize.GetFileSizeInBytes(MetricPrefix);
            var formattedFileSize = ((double)inputFileSize).FormatFileSize();

            inputFileName = Path.Combine(storageDirectory, $"{formattedFileSize}_Benchmark_Test.txt");

            // Generate benchmark test file
            GenerateTestFile();

            // Sorting and saving output file to ensure everything is working before benchmarking
            InitializeSorter();
            Sort();
            SaveOutputFile();
        }

        [Benchmark]
        public void GenerateTestFile()
        {
            generatedLinesCount = RandomFileGenerator.GenerateTestFile(inputFileName, inputFileSize);
        }

        [Benchmark]
        public void InitializeSorter()
        {
            if (generatedLinesCount > 0)
            {
                sorterHelper = new AlphanumericSorterHelper(inputFileName);
                outputFileName = sorterHelper.OutputFileName;
            }
        }

        [Benchmark]
        public void Sort()
        {
            sorterHelper?.Sort();
        }

        [Benchmark]
        public void SaveOutputFile()
        {
            sorterHelper?.SaveOutputFile();
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
