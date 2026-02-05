namespace MultiSorterLib
{
    /// <summary>
    /// Represents an entity composed of a string and numeric component, supporting comparison and equality operations
    /// based on both parts.
    /// </summary>
    /// <remarks>AlphanumericEntity instances are typically used to model identifiers or keys that combine a
    /// textual prefix with a numeric value, such as hierarchical codes or composite labels. Instances are immutable and
    /// can be compared or used as dictionary keys. Comparison is performed first on the string part using ordinal
    /// comparison, then on the numeric part if the string parts are equal.</remarks>
    public class AlphanumericEntity : IComparable<AlphanumericEntity>, IEquatable<AlphanumericEntity>
    {
        /// <summary>
        /// Represents the character used to separate segments in a hierarchical identifier.
        /// </summary>
        public const char Delimiter = '.';

        /// <summary>
        /// Represents the composite format string used to construct a line with two leading elements and a value.
        /// </summary>
        /// <remarks>This pattern is intended for use with string formatting methods such as
        /// string.Format, where the placeholders correspond to specific values to be inserted. The first and second
        /// placeholders typically represent prefix elements, and the third represents the main value.</remarks>
        public const string LinePattern = "{0}{1} {2}";
        
        /// <summary>
        /// Initializes a new instance of the AlphanumericEntity class with the specified string and numeric components.
        /// </summary>
        /// <param name="stringPart">The string component of the entity. Cannot be null.</param>
        /// <param name="numericPart">The numeric component of the entity.</param>
        public AlphanumericEntity(string stringPart, int numericPart)
        {
            StringPart = stringPart;
            NumericPart = numericPart;
        }

        /// <summary>
        /// Gets the string segment represented by this part.
        /// </summary>
        public string StringPart { get; }

        /// <summary>
        /// Gets the numeric component associated with this instance.
        /// </summary>
        public int NumericPart { get; }

        /// <summary>
        /// Gets the formatted entity line composed of the numeric part, delimiter, and string part.
        /// </summary>
        public string EntityLine => string.Format(LinePattern, NumericPart, Delimiter, StringPart);
        
        /// <summary>
        /// Compares the current AlphanumericEntity with another AlphanumericEntity and returns an integer that
        /// indicates their relative order.
        /// </summary>
        /// <remarks>Comparison is performed first on the string part using ordinal comparison, and then
        /// on the numeric part if the string parts are equal. If <paramref name="other"/> is null, the current instance
        /// is considered greater.</remarks>
        /// <param name="other">The AlphanumericEntity to compare with the current instance. Can be null.</param>
        /// <returns>A value less than zero if the current instance precedes <paramref name="other"/> in the sort order; zero if
        /// they are equal; or a value greater than zero if the current instance follows <paramref name="other"/>.</returns>
        public int CompareTo(AlphanumericEntity? other)
        {
            if (other is null)
            {
                return -1;
            }

            int strComparison = string.Compare(StringPart, other.StringPart, StringComparison.Ordinal);

            return strComparison != 0 ? strComparison : NumericPart.CompareTo(other.NumericPart);
        }

        /// <summary>
        /// Determines whether the current instance is equal to the specified <see cref="AlphanumericEntity"/>.
        /// </summary>
        /// <param name="other">The <see cref="AlphanumericEntity"/> to compare with the current instance. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the current instance and <paramref name="other"/> have the same numeric and string
        /// parts; otherwise, <see langword="false"/>.</returns>
        public bool Equals(AlphanumericEntity? other)
        {
            return other is not null &&
                   NumericPart.Equals(other.NumericPart) &&
                   (StringPart.Equals(other.StringPart));
        }

        /// <summary>
        /// Serves as the default hash function for the current object.
        /// </summary>
        /// <remarks>The hash code is based on the values of the StringPart and NumericPart properties.
        /// Objects that are equal will return the same hash code.</remarks>
        /// <returns>A 32-bit signed integer hash code that represents the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(StringPart, NumericPart);
        }
    }
}