using System.Globalization;

namespace MultiSorterLib.Tests
{
    // Runs once before any tests in this assembly and once after all tests
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            // Enforce invariant culture for deterministic formatting in tests
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // Initialize shared test resources if needed
            // e.g., temp directories, common mocks, configuration
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Clean up shared resources
            // e.g., delete temp directories, dispose services
        }
    }
}