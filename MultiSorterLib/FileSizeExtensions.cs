namespace MultiSorterLib
{
    /// <summary>
    /// Provides extension methods for formatting and converting file sizes using metric prefixes.
    /// </summary>
    /// <remarks>This class includes methods to represent file sizes in human-readable formats (such as KB,
    /// MB, GB) and to convert between formatted sizes and their byte values. The metric prefixes used follow the
    /// standard binary multiples (e.g., Kilo for 1024, Mega for 1024^2). These methods are intended to simplify
    /// displaying and working with file sizes in applications that require user-friendly representations.</remarks>
    public static class FileSizeExtensions
    {
        /// <summary>
        /// Specifies the standard metric unit prefixes used to denote orders of magnitude in measurements.
        /// </summary>
        /// <remarks>These prefixes represent powers of ten commonly used in scientific, engineering, and
        /// data contexts to indicate multiples of a base unit. For example, 'Kilo' represents 10^3 (1,000), 'Mega'
        /// represents 10^6 (1,000,000), and so on up to 'Yotta' for 10^24. Use this enumeration to clearly indicate the
        /// scale of a measurement or quantity when working with metric units.</remarks>
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

        /// <summary>
        /// Specifies the type of estimated size to use when calculating or reporting values that can vary, such as
        /// minimum, average, or maximum sizes.
        /// </summary>
        /// <remarks>Use this enumeration to indicate whether a minimum, average, or maximum estimated
        /// size should be considered in operations that support size estimation. The values correspond to common
        /// estimation strategies and may affect the results returned by methods or properties that utilize this
        /// type.</remarks>
        public enum EstimatedSizeType
        {
            Min = -1,
            Avg = 0,
            Max = 1
        }

        /// <summary>
        /// Represents the multiplier used for binary calculations, equal to 1,024.
        /// </summary>
        private const double BinaryMultiplier = 1024d;

        /// <summary>
        /// Formats a file size value as a human-readable string using binary metric prefixes (e.g., KB, MB, GB).
        /// </summary>
        /// <remarks>This method uses binary (base-1024) prefixes (e.g., KB = 1024 bytes). The formatted
        /// string includes up to two decimal places. If fileSize is less than 1024, the value is shown in bytes. If
        /// metricBenchmark is set to a value other than None, formatting begins at that prefix level.</remarks>
        /// <param name="fileSize">The file size to format, in bytes. Must be a non-negative value.</param>
        /// <param name="showBytes">true to append the exact byte count in parentheses; otherwise, false.</param>
        /// <param name="metricBenchmark">The initial metric prefix to use for formatting. Use MetricPrefixes.None to start with bytes.</param>
        /// <returns>A string representing the formatted file size with the appropriate metric prefix. If showBytes is true and a
        /// metric prefix is used, the exact byte count is included in parentheses.</returns>
        public static string FormatFileSize(
            this double fileSize,
            bool showBytes = false,
            MetricPrefixes metricBenchmark = MetricPrefixes.None)
        {
            string suffix = showBytes ? $" ({fileSize.GetFileSizeInBytes(metricBenchmark):##,###} B)" : string.Empty;

            while (fileSize * 2 > BinaryMultiplier)
            {
                metricBenchmark++;
                fileSize /= BinaryMultiplier;
            }

            return $"{fileSize:##,##0.##} {(metricBenchmark != MetricPrefixes.None ? metricBenchmark.ToString()[0] : string.Empty)}B" +
                   $"{(metricBenchmark > MetricPrefixes.None ? suffix : string.Empty)}";
        }

        /// <summary>
        /// Converts a file size value to its equivalent size in bytes using the specified metric prefix.
        /// </summary>
        /// <remarks>This method is useful for converting file sizes specified in larger units (such as
        /// kilobytes or megabytes) to their byte representation. The calculation uses binary multipliers (powers of
        /// 1024) based on the specified metric prefix.</remarks>
        /// <param name="fileSize">The file size value to convert. Represents the size in units determined by the metric prefix.</param>
        /// <param name="metricBenchmark">The metric prefix that indicates the unit of the file size value (for example, kilobytes, megabytes, etc.).
        /// Use MetricPrefixes.None to indicate bytes.</param>
        /// <returns>The file size in bytes as a 64-bit integer.</returns>
        public static ulong GetFileSizeInBytes(this double fileSize, MetricPrefixes metricBenchmark = MetricPrefixes.None)
        {
            return (ulong)(fileSize * Math.Pow(BinaryMultiplier, (int)metricBenchmark - 1));
        }

        /// <summary>
        /// Converts the specified file size, in bytes, to a value expressed in the given metric unit.
        /// </summary>
        /// <remarks>Use this method to obtain a file size in units such as kilobytes, megabytes, or
        /// gigabytes, based on the metric prefix provided. The conversion uses binary multiples (e.g., 1024 for
        /// kilobyte).</remarks>
        /// <param name="fileSize">The file size in bytes to convert.</param>
        /// <param name="metricBenchmark">The metric unit to which the file size should be converted. Specify a value from the <see
        /// cref="MetricPrefixes"/> enumeration. If <see cref="MetricPrefixes.None"/> is provided, the result is in
        /// bytes.</param>
        /// <returns>A double representing the file size in the specified metric unit.</returns>
        public static double GetFileSizeInUnits(this ulong fileSize, MetricPrefixes metricBenchmark = MetricPrefixes.None)
        {
            return fileSize / Math.Pow(BinaryMultiplier, (int)metricBenchmark - 1);
        }
    }
}
