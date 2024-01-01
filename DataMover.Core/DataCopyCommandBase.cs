using BDMCommandLine;
using System.Data;

namespace DataMover.Core
{
	public class DataCopyCommandBase(
				string name, string description, string usage, string example,
				string[] aliases, ICommandArgument[] arguments,
				DataLayerBase sourceDataLayer, DataLayerBase targetDataLayer
		) : DataMoverCommandBase(name, description, usage, example, aliases, arguments)
	{
		public DataLayerBase SourceDataLayer { get; set; } = sourceDataLayer;
		public DataLayerBase TargetDataLayer { get; set; } = targetDataLayer;
		public List<DatabaseTableColumn> SourceColumns { get; set; } = [];
		public List<DatabaseTableColumn> TargetColumns { get; set; } = [];
		public List<DatabaseTableColumnMapping> Mappings { get; set; } = [];
		public ColumnMatchingMethod MatchingMethod { get; set; } = ColumnMatchingMethod.Name;
		public bool TruncateTarget { get; set; } = false;

		public override void Execute()
		{
			try
			{
				base.LoggingLevel = base.Arguments.GetEnumValue<LoggingLevel>("LoggingLevel");
				this.SourceDataLayer.LoggingLevel = base.LoggingLevel;
				this.TargetDataLayer.LoggingLevel = base.LoggingLevel;
				this.MatchingMethod = base.Arguments.GetEnumValue<ColumnMatchingMethod>("ColumnMatchingMethod");
				this.TruncateTarget = base.Arguments.GetFlagValue("TruncateTarget");

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
						ConsoleText.Red(commandArgument.ToString() ?? string.Empty),
						ConsoleText.BlankLine()
					);
				}
				this.SourceColumns = this.SourceDataLayer.GetColumns();
				this.TargetColumns = this.TargetDataLayer.GetColumns();
				if (this.TargetColumns.Count == 0)
				{
					this.TargetDataLayer.SetColumns(this.SourceColumns);
					this.TargetColumns = this.TargetDataLayer.GetColumns();
				}
				switch (MatchingMethod)
				{
					case ColumnMatchingMethod.Position:
						foreach (DatabaseTableColumn targetColumn in this.TargetColumns)
							this.Mappings.Add(new(
								targetColumn,
								this.SourceColumns.Find(c => c.Postion.Equals(targetColumn.Postion))
							));
						break;
					case ColumnMatchingMethod.Name:
					default:
						foreach (DatabaseTableColumn targetColumn in this.TargetColumns)
							this.Mappings.Add(new(
								targetColumn,
								this.SourceColumns.Find(c => c.Name.Equals(targetColumn.Name, StringComparison.CurrentCultureIgnoreCase))
							));
						break;
				}
				if (this.Mappings.Any(m => m.Source == null))
					throw new MissingSourceColumnException(this.MatchingMethod, this.Mappings.Where(m => m.Source == null));

				base.WriteOutput(LogLevel.Information,
					ConsoleText.BlankLine(),
					ConsoleText.Blue($"Column mappings based on \"{this.MatchingMethod}\" matching."),
					ConsoleText.BlankLine()
				);
				foreach (DatabaseTableColumnMapping mapping in this.Mappings)
					base.WriteOutput(LogLevel.Information,
						ConsoleText.Default("   "),
						ConsoleText.Yellow($"{mapping.Source?.Name} ({mapping.Source?.Postion})"),
						ConsoleText.Gray(" => "),
						ConsoleText.Red($"{mapping.Target.Name} ({mapping.Target.Postion})"),
						ConsoleText.BlankLine()
					);
				DataTable sourceDataTable = this.SourceDataLayer.GetDataTable();
				if (this.TruncateTarget)
				{
					base.WriteOutput(LogLevel.Information,
						ConsoleText.Blue($"Truncating {this.TargetDataLayer.QualifiedObjectName}."),
						ConsoleText.BlankLine()
					);
					this.TargetDataLayer.Truncate();
				}
				if (sourceDataTable.Rows.Count > 0)
				{
					base.WriteOutput(LogLevel.Information,
						ConsoleText.Blue($"Writing {sourceDataTable.Rows.Count} from \"{this.SourceDataLayer.QualifiedObjectName}\" to \"{this.TargetDataLayer.QualifiedObjectName}\"."),
						ConsoleText.BlankLine()
					);
					this.TargetDataLayer.WriteDataTable(sourceDataTable, this.Mappings);
				}
				else
					base.WriteOutput(LogLevel.Information,
						ConsoleText.Red($"No rows returned from {this.SourceDataLayer.QualifiedObjectName}."),
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
