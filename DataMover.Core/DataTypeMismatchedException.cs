namespace DataMover.Core
{
    public class DataTypeMismatchedException : Exception
    {
        public DataTypeMismatchedException() { }

        public DataTypeMismatchedException(string message)
            : base(message) { }

        public DataTypeMismatchedException(string message, Exception inner)
            : base(message, inner) { }

        public DataTypeMismatchedException(string sourceColumn, string sourceDataType, string targetColumn, string targetDataType)
            : this($"Source \"{sourceColumn} with a data type of {sourceDataType} cannot be inserted into {targetColumn} with a data type of {targetDataType}.")
        { }
    }
}
