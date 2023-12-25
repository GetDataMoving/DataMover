using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class PostgreSQLQueryExecute : QueryExecuteCommandBase
	{
		public PostgreSQLQueryExecute()
			: base(
				"PostgreSQLQueryExecute",
				"Used to execute a query on PostgreSQL.",
				"PostgreSQLQueryExecute [pgqe] {arguments}",
				(
					"DataMover pgqe "
					+ "-pgConn \"{PostgreSQLConnString}\" "
					+ "-query \"{Query}\""
				),
				["pgqe"],
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
