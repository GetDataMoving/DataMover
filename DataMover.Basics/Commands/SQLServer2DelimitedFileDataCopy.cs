using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class SQLServer2DelimitedFileDataCopy : DataCopyCommandBase
	{
		public SQLServer2DelimitedFileDataCopy()
			: base(
				"SQLServer2DelimitedFile",
				"Used to copy data from a SQL Server Table or View to a Delimited File.",
				"SQLServer2DelimitedFile [ss2df] {arguments}",
				(
					"SQLServer2DelimitedFile ss2df "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-srcSchema \"{SourceSchema}\" "
					+ "-srcTable \"{SourceTableOrView}\" "
					+ "-dfPath \"{PathToFile}\" "
					+ "-cd \",\" "
					+ "-head "
					+ "-cols \"Column1,Column2\""
				),
				["ss2df"],
				new CommandArgumentBase[] {
					//Source
					CommandArguments.SQLServerConnectionString,
					CommandArguments.SourceSchema,
					CommandArguments.SourceTable,

					//Target
					CommandArguments.DelimitedFilePath,
					CommandArguments.ColumnDelimiter,
					CommandArguments.HasHeaderRow,
					CommandArguments.Columns,

					//Other
					CommandArguments.ColumnMatchingMethod,
					CommandArguments.TruncateTarget,
					CommandArguments.LoggingLevel
				},
				new SQLServerDataLayer(), new DelimitedFileDataLayer()
			)
		{ }

		public override void Execute()
		{
			base.SourceDataLayer = new SQLServerDataLayer(
				base.Arguments.GetSimpleValue("SQLServerConnectionString"),
				base.Arguments.GetSimpleValue("SourceSchema"),
				base.Arguments.GetSimpleValue("SourceTable")
			);
			if (base.Arguments.GetArrayValue("Columns").Length > 0)
				base.TargetDataLayer = new DelimitedFileDataLayer(
					base.Arguments.GetSimpleValue("DelimitedFilePath"),
					base.Arguments.GetSimpleValue("ColumnDelimiter"),
					base.Arguments.GetFlagValue("HasHeaderRow"),
					base.Arguments.GetArrayValue("Columns")
				);
			else
				base.TargetDataLayer = new DelimitedFileDataLayer(
					base.Arguments.GetSimpleValue("DelimitedFilePath"),
					base.Arguments.GetSimpleValue("ColumnDelimiter"),
					base.Arguments.GetFlagValue("HasHeaderRow")
				);
			base.Execute();
		}
	}
}
