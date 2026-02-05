using MultiSorterLib;

namespace TestFileGenerator.Tests
{
    [TestFixture]
    public class FileSizeExtensionsTests
    {
        [Test]
        public void FormatFileSize_BytesUnder1024_ShowsBytesSuffixWithoutUnit()
        {
            double size = 512;
            string formatted = FileSizeExtensions.FormatFileSize(size);
            Assert.That(formatted, Is.EqualTo("512 B"));
        }

        [Test]
        public void FormatFileSize_StartingAtMega_ShowsMBAndOptionalBytes()
        {
            double size = 1; // 1 MB when benchmark is Mega
            string formatted = FileSizeExtensions.FormatFileSize(size, showBytes: true, metricBenchmark: FileSizeExtensions.MetricPrefixes.Mega);
            Assert.That(formatted, Does.Contain("MB"));
            Assert.That(formatted, Does.Contain("(1,048,576 B)"));
        }

        [Test]
        public void GetFileSizeInBytes_MetricConversions_AreCorrect()
        {
            // None: bytes
            Assert.That(FileSizeExtensions.GetFileSizeInBytes(1024d, FileSizeExtensions.MetricPrefixes.None), Is.EqualTo(1024L));

            // Kilo: 1 * 1024^1
            Assert.That(FileSizeExtensions.GetFileSizeInBytes(1d, FileSizeExtensions.MetricPrefixes.Kilo), Is.EqualTo(1024L));

            // Mega: 2 * 1024^2
            Assert.That(FileSizeExtensions.GetFileSizeInBytes(2d, FileSizeExtensions.MetricPrefixes.Mega), Is.EqualTo(2L * 1024L * 1024L));

            // Giga: 1.5 GB -> floor to long via cast
            long expected = (long)(1.5d * Math.Pow(1024d, (int)FileSizeExtensions.MetricPrefixes.Giga - 1));
            Assert.That(FileSizeExtensions.GetFileSizeInBytes(1.5d, FileSizeExtensions.MetricPrefixes.Giga), Is.EqualTo(expected));
        }
    }
}