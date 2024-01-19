using BDMCommandLine;
using DataMover.Basics.DataLayers;
using DataMover.Core;

namespace DataMover.Basics.Commands
{
	public class DelimitedFile2PostgreSQLDataCopy : DataCopyCommandBase
	{
		public DelimitedFile2PostgreSQLDataCopy()
			: base(
				"DelimitedFile2PostgreSQL",
				"Used to copy data from a Delimited File to a PostgreSQL Table.",
				"DelimitedFile2PostgreSQL [df2pg] {arguments}",
				(
					"DelimitedFile2SQLServer df2ss "
					+ "-dfPath \"{PathToFile}\" "
					+ "-cd \",\" "
					+ "-head "
					+ "-cols \"Column1,Column2\" "
					+ "-ssConn \"{PostgreSQLConnString}\" "
					+ "-srcSchema \"{TargetSchema}\" "
					+ "-srcTable \"{TargetTable}\""
				),
				["df2pg"],
				new CommandArgumentBase[] {
					//Source
					CommandArguments.DelimitedFilePath,
					CommandArguments.ColumnDelimiter,
					CommandArguments.HasHeaderRow,
					CommandArguments.Columns,

					//Target
					CommandArguments.PostgreSQLConnectionString,
					CommandArguments.TargetSchema,
					CommandArguments.TargetTable,

					//Other
					CommandArguments.ColumnMatchingMethod,
					CommandArguments.TruncateTarget,
					CommandArguments.LoggingLevel
				},
				new DelimitedFileDataLayer(), new PostgreSQLDataLayer()
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
			base.TargetDataLayer = new PostgreSQLDataLayer(
				base.Arguments.GetSimpleValue("PostgreSQLConnectionString"),
				base.Arguments.GetSimpleValue("TargetSchema"),
				base.Arguments.GetSimpleValue("TargetTable")
			);
			base.Execute();
		}
	}
}
