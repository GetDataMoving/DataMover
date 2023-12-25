namespace DataMover.Core
{
    public class MissingSourceColumnException : Exception
    {
        public MissingSourceColumnException() { }

        public MissingSourceColumnException(string message)
            : base(message) { }

        public MissingSourceColumnException(string message, Exception inner)
            : base(message, inner) { }

        public MissingSourceColumnException(ColumnMatchingMethod matchingMethod, IEnumerable<DatabaseTableColumnMapping> mappings)
            : this($"Source columns for the following target columns could not be found based on \"{matchingMethod}\" matching.\n   {string.Join("\n   ", mappings)}")
        { }
    }
}
