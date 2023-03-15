namespace MultiSorterLib
{
    public class AlphanumericEntity : IComparable<AlphanumericEntity>, IEquatable<AlphanumericEntity>
    {
        public const char Delimiter = '.';
        public const string LinePattern = "{0}{1} {2}";
        
        public AlphanumericEntity(string stringPart, int numericPart)
        {
            StringPart = stringPart;
            NumericPart = numericPart;
        }

        public string StringPart { get; }

        public int NumericPart { get; }

        public string EntityLine => string.Format(LinePattern, NumericPart, Delimiter, StringPart);
        
        public int CompareTo(AlphanumericEntity? other)
        {
            if (other is null)
            {
                return -1;
            }

            int strComparison = string.Compare(StringPart, other.StringPart, StringComparison.Ordinal);

            return strComparison != 0 ? strComparison : NumericPart.CompareTo(other.NumericPart);
        }

        public bool Equals(AlphanumericEntity? other)
        {
            return other is not null &&
                   NumericPart.Equals(other.NumericPart) &&
                   (StringPart.Equals(other.StringPart));
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(StringPart, NumericPart);
        }
    }
}