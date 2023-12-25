using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
    public class SQLServer2PostgreSQLDataCopy : DataCopyCommandBase
    {
        public SQLServer2PostgreSQLDataCopy()
            : base(
                "SQLServer2PostgreSQL",
                "Used to copy data from a SQL Server Table or View to a PostgreSQL Table.",
                "SQLServer2PostgreSQL [ss2pg] {arguments}",
				(
					"DataMover ss2pg "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-srcSchema \"{SourceSchema}\" "
					+ "-srcTable \"{SourceTableOrView}\" "
					+ "-pgConn \"{PostgreSQLConnString}\" "
					+ "-trgSchema \"{TargetSchema}\" "
					+ "-trgTable \"{TargetTable}\""
				),
                ["ss2pg"],
                new CommandArgumentBase[] {
					//Source
					CommandArguments.SQLServerConnectionString,
					CommandArguments.SourceSchema,
					CommandArguments.SourceTable,

					//Target
					CommandArguments.PostgreSQLConnectionString,
					CommandArguments.TargetSchema,
					CommandArguments.TargetTable,

					//Other
					CommandArguments.ColumnMatchingMethod,
					CommandArguments.TruncateTarget,
					CommandArguments.LoggingLevel
                },
                new SQLServerDataLayer(), new PostgreSQLDataLayer()
            )
        { }

        public override void Execute()
        {
            base.SourceDataLayer = new SQLServerDataLayer(
				base.Arguments.GetSimpleValue("SQLServerConnectionString"),
				base.Arguments.GetSimpleValue("SourceSchema"),
				base.Arguments.GetSimpleValue("SourceTable")
			);
			base.TargetDataLayer = new PostgreSQLDataLayer(
				base.Arguments.GetSimpleValue("PostgreSQLConnectionString"),
				base.Arguments.GetSimpleValue("TargetSchema"),
				base.Arguments.GetSimpleValue("TargetTable")
			);
			base.Execute();
        }
    }
}
