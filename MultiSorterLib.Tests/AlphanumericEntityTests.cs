using MultiSorterLib;

namespace TestFileGenerator.Tests
{
    [TestFixture]
    public class AlphanumericEntityTests
    {
        [Test]
        public void EntityLine_UsesPattern_NumberDelimiterString()
        {
            var e = new AlphanumericEntity("Alpha Beta", 42);
            Assert.That(e.EntityLine, Is.EqualTo($"{42}{AlphanumericEntity.Delimiter} Alpha Beta"));
        }

        [Test]
        public void CompareTo_OrdersByStringThenNumber()
        {
            var a = new AlphanumericEntity("A", 1);
            var b = new AlphanumericEntity("A", 2);
            var c = new AlphanumericEntity("B", 0);

            Assert.That(a.CompareTo(b), Is.LessThan(0));   // same string, numeric 1 < 2
            Assert.That(b.CompareTo(c), Is.LessThan(0));   // "A" < "B"
            Assert.That(c.CompareTo(a), Is.GreaterThan(0));
            Assert.That(a.CompareTo(null), Is.LessThan(0)); // other is null -> current greater? Implementation returns -1
        }

        [Test]
        public void Equals_TrueForSameNumericAndString()
        {
            var a = new AlphanumericEntity("X", 5);
            var b = new AlphanumericEntity("X", 5);
            var c = new AlphanumericEntity("X", 6);
            var d = new AlphanumericEntity("Y", 5);

            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.Equals(c), Is.False);
            Assert.That(a.Equals(d), Is.False);
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void GetHashCode_SameForEqualEntities()
        {
            var a = new AlphanumericEntity("Z", 9);
            var b = new AlphanumericEntity("Z", 9);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }
    }
}