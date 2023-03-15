using BenchmarkDotNet.Attributes;
using MultiSorterLib;

namespace AlphanumericSorterBenchmark
{
    [MemoryDiagnoser()]
    public class BenchmarkSorter : IDisposable
    {
        private const string InputFileName = @"C:\AltiumTestProject\TempStorage\1GB.txt";
        private const string OutputFileName = @"C:\AltiumTestProject\TempStorage\1GB_OUT.txt";

        private AlphanumericSorterHelper? sorterHelper;

        public BenchmarkSorter()
        {
            Initialize();
            Sort();
            SaveToFile();
        }

        [Benchmark]
        public void Initialize()
        {
            sorterHelper = AlphanumericSorterHelper.LoadFromFile(InputFileName);
        }

        [Benchmark]
        public void Sort()
        {
            sorterHelper?.Sort(false);
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
