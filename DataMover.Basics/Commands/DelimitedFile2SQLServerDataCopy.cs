using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class DelimitedFile2SQLServerDataCopy : DataCopyCommandBase
	{
		public DelimitedFile2SQLServerDataCopy()
			: base(
				"DelimitedFile2SQLServer",
				"Used to copy data from a Delimited File to a SQL Server Table.",
				"DelimitedFile2SQLServer [df2ss] {arguments}",
				(
					"DelimitedFile2SQLServer df2ss "
					+ "-dfPath \"{PathToFile}\" "
					+ "-cd \",\" "
					+ "-head "
					+ "-cols \"Column1,Column2\" "
					+ "-ssConn \"{SQLServerConnString}\" "
					+ "-trgSchema \"{SourceSchema}\" "
					+ "-trgTable \"{SourceTableOrView}\""
				),
				["df2ss"],
				new CommandArgumentBase[] {
					//Source
					CommandArguments.DelimitedFilePath,
					CommandArguments.ColumnDelimiter,
					CommandArguments.HasHeaderRow,
					CommandArguments.Columns,
					
					//Target
					CommandArguments.SQLServerConnectionString,
					CommandArguments.TargetSchema,
					CommandArguments.TargetTable,

					//Other
					CommandArguments.ColumnMatchingMethod,
					CommandArguments.TruncateTarget,
					CommandArguments.LoggingLevel
				},
				new DelimitedFileDataLayer(), new SQLServerDataLayer()
			)
		{ }

		public override void Execute()
		{
			if (base.Arguments.GetArrayValue("Columns").Length > 0)
				base.SourceDataLayer = new DelimitedFileDataLayer(
					base.Arguments.GetSimpleValue("DelimitedFilePath"),
					base.Arguments.GetSimpleValue("ColumnDelimiter"),
					base.Arguments.GetFlagValue("HasHeaderRow"),
					base.Arguments.GetArrayValue("Columns")
				);
			else
				base.SourceDataLayer = new DelimitedFileDataLayer(
					base.Arguments.GetSimpleValue("DelimitedFilePath"),
					base.Arguments.GetSimpleValue("ColumnDelimiter"),
					base.Arguments.GetFlagValue("HasHeaderRow")
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
