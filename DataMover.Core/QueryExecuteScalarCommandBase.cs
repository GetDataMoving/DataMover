using BDMCommandLine;

namespace DataMover.Core
{
    public class QueryExecuteScalarCommandBase(
				String name, String description, String usage, String example,
				String[] aliases, ICommandArgument[] arguments,
				DataLayerBase dataLayer
		) : DataMoverCommandBase(name, description, usage, example, aliases, arguments)
    {
		public DataLayerBase DataLayer { get; set; } = dataLayer;
		public String Query { get; set; } = String.Empty;

		public override void Execute()
        {
			try
			{
				base.LoggingLevel = base.Arguments.GetEnumValue<LoggingLevel>("LoggingLevel");
				this.Query = base.Arguments.GetSimpleValue("Query");
				base.WriteOutput(LogLevel.Information,
						ConsoleText.Default("Command: "),
						ConsoleText.Red(base.Name),
						ConsoleText.BlankLine(),
						ConsoleText.Default("Arguments:"),
						ConsoleText.BlankLine()
				);
				foreach (ICommandArgument commandArgument in base.Arguments)
				{
					base.WriteOutput(LogLevel.Information,
						ConsoleText.Default("   "),
						ConsoleText.Yellow(commandArgument.Name),
						ConsoleText.Gray(" = "),
						ConsoleText.Red(commandArgument.ToString() ?? String.Empty),
						ConsoleText.BlankLine()
					);
				}

				if (String.IsNullOrEmpty(this.Query))
					throw new EmptyQueryException();
				base.WriteOutput(LogLevel.Information,
					ConsoleText.Blue($"Executing query on {this.DataLayer.QualifiedDatabaseName}..."),
					ConsoleText.BlankLine(),
					ConsoleText.Green(this.Query),
					ConsoleText.BlankLine()
				);
				String result = this.DataLayer.ExecuteScalar(this.Query, null);
				CommandLine.OutputTextCollection(
					ConsoleText.Default("BEGIN RESULTS:"),
					ConsoleText.BlankLine(),
					ConsoleText.Default(result),
					ConsoleText.BlankLine(),
					ConsoleText.Default("END RESULTS:"),
					ConsoleText.BlankLine()
				);
			}
			catch (Exception exception)
			{
				base.WriteOutput(LogLevel.Exception,
					ConsoleText.Red("\n*****************************************\n"),
					ConsoleText.Red(exception.Message),
					ConsoleText.Red("\n*****************************************\n"),
					ConsoleText.BlankLine()
				);
			}
		}
	}
}
