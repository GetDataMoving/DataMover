using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class PostgreSQLQueryExecuteScalar : QueryExecuteScalarCommandBase
	{
		public PostgreSQLQueryExecuteScalar()
			: base(
				"PostgreSQLQueryExecuteScalar",
				"Used to execute a query on PostgreSQL.",
				"PostgreSQLQueryExecuteScalar [pgqes] {arguments}",
				(
					"DataMover pgqes "
					+ "-pgConn \"{PostgreSQLConnString}\" "
					+ "-query \"{Query}\""
				),
				["pgqes"],
				new CommandArgumentBase[] {
					CommandArguments.PostgreSQLConnectionString,
					CommandArguments.Query,
					CommandArguments.LoggingLevel
				},
				new PostgreSQLDataLayer()
			)
		{
		}

		public override void Execute()
		{
			base.DataLayer = new PostgreSQLDataLayer(base.Arguments.GetSimpleValue("PostgreSQLConnectionString"));
			base.Execute();
		}
	}
}
