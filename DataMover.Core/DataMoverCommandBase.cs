using BDMCommandLine;

namespace DataMover.Core
{
    public class DataMoverCommandBase(
                string name, string description, string usage, string example,
                string[] aliases, ICommandArgument[] arguments
        ) : CommandBase(name, description, usage, example, aliases, arguments)
    {
        public LoggingLevel LoggingLevel { get; set; } = LoggingLevel.Exception;

        public void WriteOutput(LogLevel logLevel, params ConsoleText[] texts)
        {
            switch (this.LoggingLevel)
            {
                case LoggingLevel.Verbose:
                    CommandLine.OutputTextCollection(texts);
                    break;
                case LoggingLevel.Exception:
                    if (logLevel == LogLevel.Exception)
                        CommandLine.OutputTextCollection(texts);
                    break;
            }
        }

        public override void Execute()
            => throw new NotImplementedException();
    }
}
