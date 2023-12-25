using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class SQLServerQueryExecute : QueryExecuteCommandBase
	{
		public SQLServerQueryExecute()
			: base(
				"SQLServerQueryExecute",
				"Used to execute a query on SQL Server.",
				"SQLServerQueryExecute [ssqe] {arguments}",
				(
					"DataMover ssqe "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-q \"{Query}\""
				),
				["ssqe"],
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
