using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class SQLServerQueryExecuteScalar : QueryExecuteScalarCommandBase
	{
		public SQLServerQueryExecuteScalar()
			: base(
				"SQLServerQueryScalarExecuteScalar",
				"Used to execute a query on SQL Server and return the scalar value.",
				"SQLServerQueryScalarExecuteScalar [ssqes] {arguments}",
				(
					"DataMover ssqes "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-q \"{Query}\""
				),
				["ssqes"],
				new CommandArgumentBase[] {
					CommandArguments.SQLServerConnectionString,
					CommandArguments.Query,
					CommandArguments.LoggingLevel
				},
				new SQLServerDataLayer()
			)
		{ }

		public override void Execute()
		{
			base.DataLayer = new SQLServerDataLayer(base.Arguments.GetSimpleValue("SQLServerConnectionString"));
			base.Execute();
		}
	}
}
