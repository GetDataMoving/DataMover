using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class PostgreSQL2DelimitedFileDataCopy : DataCopyCommandBase
	{
		public PostgreSQL2DelimitedFileDataCopy()
			: base(
				"PostgreSQL2DelimitedFile",
				"Used to copy data from a PostgreSQL Table or View to a Delimited File.",
				"PostgreSQL2DelimitedFile [pg2df] {arguments}",
				(
					"DataMover pg2df "
					+ "-pgConn \"{PostgreSQLConnString}\" "
					+ "-srcSchema \"{SourceSchema}\" "
					+ "-srcTable \"{SourceTableOrView}\" "
					+ "-dfPath \"{PathToFile}\" "
					+ "-cd \",\" "
					+ "-head "
					+ "-cols \"Column1,Column2\""
				),
				["pg2df"],
				new CommandArgumentBase[] {
					//Source
					CommandArguments.PostgreSQLConnectionString,
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
				new PostgreSQLDataLayer(), new DelimitedFileDataLayer()
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
