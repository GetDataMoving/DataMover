namespace DataMover.Core
{
    public class EmptyQueryException : Exception
    {
        public EmptyQueryException()
            : base("The query cannot be empty.") { }

        public EmptyQueryException(string message)
            : base(message) { }

        public EmptyQueryException(string message, Exception inner)
            : base(message, inner) { }
    }
}
