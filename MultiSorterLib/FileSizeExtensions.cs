namespace MultiSorterLib
{
    public static class FileSizeExtensions
    {
        public enum MetricPrefixes
        {
            None = 1,
            Kilo,
            Mega,
            Giga,
            Tera,
            Peta,
            Exa,
            Zetta,
            Yotta
        }

        public enum EstimatedSizeType
        {
            Min = -1,
            Avg = 0,
            Max = 1
        }
        
        private const double BinaryMultiplier = 1024d;

        public static string FormatFileSize(
            this double fileSize,
            bool showBytes = false,
            MetricPrefixes benchmark = MetricPrefixes.None)
        {
            string suffix = showBytes ? $" ({fileSize.GetFileSizeInBytes(benchmark):##,###} B)" : string.Empty;
            
            while (fileSize * 2 > BinaryMultiplier)
            {
                benchmark++;
                fileSize /= BinaryMultiplier;
            }

            return $"{fileSize:##,##0.##} {benchmark.ToString()[0]}B{(benchmark > MetricPrefixes.None ? suffix : string.Empty)}";
        }

        public static long GetFileSizeInBytes(this double fileSize, MetricPrefixes benchmark = MetricPrefixes.None)
        {
            return (long)(fileSize * Math.Pow(BinaryMultiplier, (int)benchmark - 1));
        }
    }
}
