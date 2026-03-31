using MultiSorterLib;
using System.Diagnostics;

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

        [Test]
        public void CompareTo_EqualEntities_ReturnsZero()
        {
            var a = new AlphanumericEntity("Same", 10);
            var b = new AlphanumericEntity("Same", 10);
            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_IsAntisymmetric()
        {
            var a = new AlphanumericEntity("Alpha", 1);
            var b = new AlphanumericEntity("Beta", 2);
            Assert.That(Math.Sign(a.CompareTo(b)), Is.EqualTo(-Math.Sign(b.CompareTo(a))));
        }

        [Test]
        public void CompareTo_IsTransitive()
        {
            var a = new AlphanumericEntity("A", 1);
            var b = new AlphanumericEntity("B", 1);
            var c = new AlphanumericEntity("C", 1);
            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(b.CompareTo(c), Is.LessThan(0));
            Assert.That(a.CompareTo(c), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_SameString_OrdersByNumericPart()
        {
            var entities = new[]
            {
                new AlphanumericEntity("X", 100),
                new AlphanumericEntity("X", 1),
                new AlphanumericEntity("X", 50),
            };
            var sorted = entities.OrderBy(e => e).ToArray();
            Assert.That(sorted[0].NumericPart, Is.EqualTo(1));
            Assert.That(sorted[1].NumericPart, Is.EqualTo(50));
            Assert.That(sorted[2].NumericPart, Is.EqualTo(100));
        }

        [Test]
        public void EntityLine_WithZeroNumericPart_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("Test", 0);
            Assert.That(e.EntityLine, Is.EqualTo($"0{AlphanumericEntity.Delimiter} Test"));
        }

        [Test]
        public void EntityLine_WithNegativeNumericPart_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("Neg", -5);
            Assert.That(e.EntityLine, Is.EqualTo($"-5{AlphanumericEntity.Delimiter} Neg"));
        }

        [Test]
        public void EntityLine_WithEmptyStringPart_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("", 7);
            Assert.That(e.EntityLine, Is.EqualTo($"7{AlphanumericEntity.Delimiter} "));
        }

        [Test]
        public void Equals_Reflexive_ReturnsTrue()
        {
            var a = new AlphanumericEntity("Ref", 99);
            Assert.That(a.Equals(a), Is.True);
        }

        [Test]
        public void Equals_IsSymmetric()
        {
            var a = new AlphanumericEntity("Sym", 3);
            var b = new AlphanumericEntity("Sym", 3);
            Assert.That(a.Equals(b), Is.EqualTo(b.Equals(a)));
        }

        [Test]
        public void GetHashCode_DifferentEntities_MayDiffer()
        {
            var a = new AlphanumericEntity("A", 1);
            var b = new AlphanumericEntity("B", 2);
            Assert.That(a.GetHashCode(), Is.Not.EqualTo(b.GetHashCode()));
        }

        [TestCase("Hello", 42)]
        [TestCase("", 0)]
        [TestCase("LongStringPartValue", int.MaxValue)]
        public void Properties_ReturnConstructorValues(string stringPart, int numericPart)
        {
            var e = new AlphanumericEntity(stringPart, numericPart);
            Assert.That(e.StringPart, Is.EqualTo(stringPart));
            Assert.That(e.NumericPart, Is.EqualTo(numericPart));
        }

        // ─── Constants ───────────────────────────────────────────────────

        [Test]
        public void Delimiter_IsDotCharacter()
        {
            Assert.That(AlphanumericEntity.Delimiter, Is.EqualTo('.'));
        }

        [Test]
        public void LinePattern_ContainsThreePlaceholders()
        {
            Assert.That(AlphanumericEntity.LinePattern, Is.EqualTo("{0}{1} {2}"));
        }

        // ─── CompareTo – ordinal case sensitivity ────────────────────────

        [Test]
        public void CompareTo_OrdinalComparison_UppercaseBeforeLowercase()
        {
            // In ordinal comparison 'A' (65) < 'a' (97)
            var upper = new AlphanumericEntity("A", 1);
            var lower = new AlphanumericEntity("a", 1);
            Assert.That(upper.CompareTo(lower), Is.LessThan(0));
            Assert.That(lower.CompareTo(upper), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_OrdinalComparison_DigitsBeforeLetters()
        {
            // '0' (48) < 'A' (65) in ordinal
            var digit = new AlphanumericEntity("0Prefix", 1);
            var letter = new AlphanumericEntity("APrefix", 1);
            Assert.That(digit.CompareTo(letter), Is.LessThan(0));
        }

        // ─── CompareTo – numeric extremes ────────────────────────────────

        [Test]
        public void CompareTo_IntMinValue_LessThanIntMaxValue()
        {
            var min = new AlphanumericEntity("X", int.MinValue);
            var max = new AlphanumericEntity("X", int.MaxValue);
            Assert.That(min.CompareTo(max), Is.LessThan(0));
            Assert.That(max.CompareTo(min), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_NegativeNumbers_OrdersCorrectly()
        {
            var neg = new AlphanumericEntity("Z", -100);
            var zero = new AlphanumericEntity("Z", 0);
            var pos = new AlphanumericEntity("Z", 100);

            Assert.That(neg.CompareTo(zero), Is.LessThan(0));
            Assert.That(zero.CompareTo(pos), Is.LessThan(0));
            Assert.That(neg.CompareTo(pos), Is.LessThan(0));
        }

        [Test]
        public void CompareTo_SameIntMinValue_ReturnsZero()
        {
            var a = new AlphanumericEntity("Edge", int.MinValue);
            var b = new AlphanumericEntity("Edge", int.MinValue);
            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        // ─── CompareTo / Equals consistency ──────────────────────────────

        [Test]
        public void CompareTo_ReturnsZero_WhenEqualsIsTrue()
        {
            var a = new AlphanumericEntity("Consistency", 42);
            var b = new AlphanumericEntity("Consistency", 42);
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a.CompareTo(b), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_ReturnsNonZero_WhenEqualsIsFalse()
        {
            var a = new AlphanumericEntity("A", 1);
            var b = new AlphanumericEntity("B", 1);
            Assert.That(a.Equals(b), Is.False);
            Assert.That(a.CompareTo(b), Is.Not.EqualTo(0));
        }

        [TestCase("Same", 1, "Same", 2)]
        [TestCase("A", 1, "B", 1)]
        [TestCase("A", 1, "B", 2)]
        public void CompareTo_NonEqual_IsNonZero(string s1, int n1, string s2, int n2)
        {
            var a = new AlphanumericEntity(s1, n1);
            var b = new AlphanumericEntity(s2, n2);
            Assert.That(a.CompareTo(b), Is.Not.EqualTo(0));
        }

        // ─── Equals – transitivity ──────────────────────────────────────

        [Test]
        public void Equals_IsTransitive()
        {
            var a = new AlphanumericEntity("Trans", 7);
            var b = new AlphanumericEntity("Trans", 7);
            var c = new AlphanumericEntity("Trans", 7);
            Assert.That(a.Equals(b), Is.True);
            Assert.That(b.Equals(c), Is.True);
            Assert.That(a.Equals(c), Is.True);
        }

        // ─── GetHashCode – idempotency ──────────────────────────────────

        [Test]
        public void GetHashCode_MultipleCallsSameInstance_ReturnsSameValue()
        {
            var e = new AlphanumericEntity("Idempotent", 123);
            int first = e.GetHashCode();
            int second = e.GetHashCode();
            int third = e.GetHashCode();
            Assert.That(first, Is.EqualTo(second));
            Assert.That(second, Is.EqualTo(third));
        }

        // ─── EntityLine – extreme / special values ───────────────────────

        [Test]
        public void EntityLine_WithIntMaxValue_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("Max", int.MaxValue);
            Assert.That(e.EntityLine, Is.EqualTo($"{int.MaxValue}{AlphanumericEntity.Delimiter} Max"));
        }

        [Test]
        public void EntityLine_WithIntMinValue_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("Min", int.MinValue);
            Assert.That(e.EntityLine, Is.EqualTo($"{int.MinValue}{AlphanumericEntity.Delimiter} Min"));
        }

        [Test]
        public void EntityLine_WithSpacesInStringPart_PreservesSpaces()
        {
            var e = new AlphanumericEntity("Hello World Foo", 5);
            Assert.That(e.EntityLine, Is.EqualTo($"5{AlphanumericEntity.Delimiter} Hello World Foo"));
        }

        [Test]
        public void EntityLine_WithSpecialCharacters_PreservesCharacters()
        {
            var e = new AlphanumericEntity("@#$%^&*()", 1);
            Assert.That(e.EntityLine, Is.EqualTo($"1{AlphanumericEntity.Delimiter} @#$%^&*()"));
        }

        [Test]
        public void EntityLine_WithUnicodeStringPart_FormatsCorrectly()
        {
            var e = new AlphanumericEntity("Ünïcödé", 99);
            Assert.That(e.EntityLine, Is.EqualTo($"99{AlphanumericEntity.Delimiter} Ünïcödé"));
        }

        [Test]
        public void EntityLine_WithVeryLongStringPart_FormatsCorrectly()
        {
            string longStr = new string('X', 10_000);
            var e = new AlphanumericEntity(longStr, 1);
            Assert.That(e.EntityLine, Does.StartWith($"1{AlphanumericEntity.Delimiter} "));
            Assert.That(e.EntityLine.Length, Is.EqualTo(3 + longStr.Length)); // "1. " + longStr
        }

        // ─── HashSet / Dictionary contract ───────────────────────────────

        [Test]
        public void HashSet_RecognisesEqualEntities_ViaIEquatableAndGetHashCode()
        {
            var set = new HashSet<AlphanumericEntity>
            {
                new("Dup", 1),
                new("Dup", 1),  // duplicate – same StringPart and NumericPart
                new("Dup", 2),  // distinct – different NumericPart
            };
            // EqualityComparer<T>.Default uses IEquatable<T>.Equals + overridden GetHashCode,
            // so the two ("Dup", 1) instances are correctly recognised as duplicates.
            Assert.That(set.Count, Is.EqualTo(2));
        }

        [Test]
        public void Dictionary_CanUseEntityAsKey_WithCustomComparer()
        {
            var comparer = new AlphanumericEntityEqualityComparer();
            var dict = new Dictionary<AlphanumericEntity, string>(comparer)
            {
                [new AlphanumericEntity("Key", 10)] = "Value1",
            };

            var lookup = new AlphanumericEntity("Key", 10);
            Assert.That(dict.ContainsKey(lookup), Is.True);
            Assert.That(dict[lookup], Is.EqualTo("Value1"));
            Assert.That(dict.ContainsKey(new AlphanumericEntity("Key", 11)), Is.False);
        }

        // ─── Helper comparer for Dictionary tests ────────────────────────

        private sealed class AlphanumericEntityEqualityComparer : IEqualityComparer<AlphanumericEntity>
        {
            public bool Equals(AlphanumericEntity? x, AlphanumericEntity? y)
            {
                if (x is null && y is null) return true;
                if (x is null || y is null) return false;
                return x.Equals(y);
            }

            public int GetHashCode(AlphanumericEntity obj) => obj.GetHashCode();
        }
    }
}