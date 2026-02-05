using System.Globalization;

namespace TestFileGenerator.Tests
{
    // Runs once before any tests in this assembly and once after all tests
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            // Example: enforce invariant culture for deterministic string/number formatting
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // TODO: initialize shared test resources/configuration here
            // e.g., create test temp folders, seed data, mock global services if applicable
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // TODO: clean up shared resources
            // e.g., delete temp folders, dispose shared services
        }
    }
}