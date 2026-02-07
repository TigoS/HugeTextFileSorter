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

        [Test]
        public void GetFileSizeInUnits_None_ReturnsSameBytes()
        {
            ulong bytes = 12345UL;
            double units = FileSizeExtensions.GetFileSizeInUnits(bytes, FileSizeExtensions.MetricPrefixes.None);
            Assert.That(units, Is.EqualTo((double)bytes));
        }

        [Test]
        public void GetFileSizeInUnits_Kilo_Mega_Giga_ReturnExpected()
        {
            // 1024 B -> 1 KB
            Assert.That(
                FileSizeExtensions.GetFileSizeInUnits(1024UL, FileSizeExtensions.MetricPrefixes.Kilo),
                Is.EqualTo(1d).Within(1e-9));

            // 1 MB in bytes -> 1 in Mega
            ulong oneMb = 1024UL * 1024UL;
            Assert.That(
                FileSizeExtensions.GetFileSizeInUnits(oneMb, FileSizeExtensions.MetricPrefixes.Mega),
                Is.EqualTo(1d).Within(1e-9));

            // 3 GB in bytes -> 3 in Giga
            ulong threeGb = 3UL * 1024UL * 1024UL * 1024UL;
            Assert.That(
                FileSizeExtensions.GetFileSizeInUnits(threeGb, FileSizeExtensions.MetricPrefixes.Giga),
                Is.EqualTo(3d).Within(1e-9));
        }

        [Test]
        public void GetFileSizeInUnits_Fractional_Kilo_ReturnsExpected()
        {
            // 1536 B -> 1.5 KB
            ulong bytes = 1536UL; // 1.5 * 1024
            double units = FileSizeExtensions.GetFileSizeInUnits(bytes, FileSizeExtensions.MetricPrefixes.Kilo);
            Assert.That(units, Is.EqualTo(1.5d).Within(1e-9));
        }

        [Test]
        public void GetFileSizeInUnits_Zero_ReturnsZero()
        {
            Assert.That(FileSizeExtensions.GetFileSizeInUnits(0UL, FileSizeExtensions.MetricPrefixes.None), Is.EqualTo(0d));
            Assert.That(FileSizeExtensions.GetFileSizeInUnits(0UL, FileSizeExtensions.MetricPrefixes.Kilo), Is.EqualTo(0d));
        }
    }
}