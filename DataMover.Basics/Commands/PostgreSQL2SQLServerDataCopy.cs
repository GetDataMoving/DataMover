using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class PostgreSQL2SQLServerDataCopy : DataCopyCommandBase
	{
		public PostgreSQL2SQLServerDataCopy()
			: base(
				"PostgreSQL2SQLServer",
				"Used to copy data from a PostgreSQL Table or View to a SQL Server Table.",
				"PostgreSQL2SQLServer [pg2ss] {arguments}",
				(
					"DataMover pg2ss "
					+ "-pgConn \"{PostgreSQLConnString}\" "
					+ "-srcSchema \"{SourceSchema}\" "
					+ "-srcTable \"{SourceTableOrView}\" "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-trgSchema \"{TargetSchema}\" "
					+ "-trgTable \"{TargetTable}\""
				),
				["pg2ss"],
				new CommandArgumentBase[] {
					//Source
					CommandArguments.PostgreSQLConnectionString,
					CommandArguments.SourceSchema,
					CommandArguments.SourceTable,
					
					//Target
					CommandArguments.SQLServerConnectionString,
					CommandArguments.TargetSchema,
					CommandArguments.TargetTable,

					//Other
					CommandArguments.ColumnMatchingMethod,
					CommandArguments.TruncateTarget,
					CommandArguments.LoggingLevel
				},
				new PostgreSQLDataLayer(), new SQLServerDataLayer()
			)
		{
		}

		public override void Execute()
		{
			base.SourceDataLayer = new PostgreSQLDataLayer(
				base.Arguments.GetSimpleValue("PostgreSQLConnectionString"),
				base.Arguments.GetSimpleValue("SourceSchema"),
				base.Arguments.GetSimpleValue("SourceTable")
			);
			base.TargetDataLayer = new SQLServerDataLayer(
				base.Arguments.GetSimpleValue("SQLServerConnectionString"),
				base.Arguments.GetSimpleValue("TargetSchema"),
				base.Arguments.GetSimpleValue("TargetTable")
			);
			base.Execute();
		}
	}
}
