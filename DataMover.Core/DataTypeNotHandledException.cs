namespace DataMover.Core
{
    public class DataTypeNotHandledException : Exception
    {
        public DataTypeNotHandledException() { }

        public DataTypeNotHandledException(string message)
            : base(message) { }

        public DataTypeNotHandledException(string message, Exception inner)
            : base(message, inner) { }

        public DataTypeNotHandledException(string targetColumn, string targetDataType)
            : this($"Target \"{targetColumn}\" with a data type of \"{targetDataType}\" cannot be handled.")
        { }
    }
}
