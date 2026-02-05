namespace MultiSorterLib.Tests
{
    [TestFixture]
    public class SystemMemoryHelperTests
    {
        [Test]
        public void GetAvailablePhysicalMemoryBytes_ReturnsNonNegative()
        {
            ulong bytes = SystemMemoryHelper.GetAvailablePhysicalMemoryBytes();
            Assert.That(bytes, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetAvailablePhysicalMemoryBytes_OnWindows_ReturnsReasonableValue()
        {
            if (!OperatingSystem.IsWindows())
            {
                Assert.Pass("Skipped: Windows-only assertion.");
            }

            ulong bytes = SystemMemoryHelper.GetAvailablePhysicalMemoryBytes();
            const ulong minReasonable = 64UL * 1024 * 1024; // 64 MB
            Assert.That(bytes, Is.GreaterThan(minReasonable));
        }

        [Test]
        public void GetAvailablePhysicalMemoryBytes_OnNonWindows_ReturnsNonNegative()
        {
            if (OperatingSystem.IsWindows())
            {
                Assert.Pass("Skipped: non-Windows-only assertion.");
            }

            ulong bytes = SystemMemoryHelper.GetAvailablePhysicalMemoryBytes();
            Assert.That(bytes, Is.GreaterThanOrEqualTo(0));
        }
    }
}
