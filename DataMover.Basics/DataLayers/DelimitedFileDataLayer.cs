using DataMover.Core;
using DataMover.Core.Descriptors;
using Microsoft.VisualBasic.FileIO;
using System.Data;

namespace DataMover.Basics.DataLayers
{
    public class DelimitedFileDataLayer : DataLayerBase
	{
		public String FilePath => base.ConnectionString;
		public String ColumnDelimiter { get; set; } = ",";
		public Boolean HasHeaderRow { get; set; } = true;
		//public List<DatabaseTableColumn>? ProvidedColumns { get; set; }
		public List<DatabaseTableColumn>? Columns { get; set; } = [];

		public DelimitedFileDataLayer()
			: base("DelimitedFile", "Provides access to methods for delimited files.")
		{ }

		public DelimitedFileDataLayer(String filePath)
			: base("DelimitedFile", "Provides access to methods for delimited files.", filePath)
			=> base.SetQuailifiedNames(Path.GetDirectoryName(filePath), filePath);

		public DelimitedFileDataLayer(String filePath, String columnDelimiter, Boolean hasHeaderRow)
			: base("DelimitedFile", "Provides access to methods for delimited files.", filePath)
		{
			base.SetQuailifiedNames(Path.GetDirectoryName(filePath), filePath);
			this.ColumnDelimiter = columnDelimiter switch
			{
				"\\tab" or "\\t" or "`t" or "^t" or "tab" or "{tab}" => "\t",
				_ => columnDelimiter,
			};
			this.HasHeaderRow = hasHeaderRow;
		}

		public DelimitedFileDataLayer(String filePath, String columnDelimiter, Boolean hasHeaderRow, String[] columns)
			: base("DelimitedFile", "Provides access to methods for delimited files.", filePath)
		{
			base.SetQuailifiedNames(Path.GetDirectoryName(filePath), filePath);
			this.ColumnDelimiter = columnDelimiter switch
			{
				"\\tab" or "\\t" or "`t" or "^t" or "tab" or "{tab}" => "\t",
				_ => columnDelimiter,
			};
			this.HasHeaderRow = hasHeaderRow;
			Int32 position = 1;
			this.Columns = [];
			foreach (String column in columns)
				this.Columns.Add(new((position ++), column, "String"));
		}

		public override List<DatabaseTableColumn> GetColumns()
		{
			if (this.Columns is null || this.Columns.Count.Equals(0))
			{
				this.Columns ??= [];
				if (File.Exists(this.FilePath))
				{
					this.Columns.Clear();
					if (this.HasHeaderRow)
					{
						using TextFieldParser textFieldParser = new(this.FilePath);
						textFieldParser.TextFieldType = FieldType.Delimited;
						textFieldParser.Delimiters = [this.ColumnDelimiter];
						Boolean isFirstRow = true;
						while (!textFieldParser.EndOfData && isFirstRow)
						{
							isFirstRow = false;
							String[]? headerRow = textFieldParser.ReadFields();
							if (headerRow is not null)
								for (Int32 columnIndex = 0; columnIndex < headerRow.Length; columnIndex++)
									this.Columns.Add(new DatabaseTableColumn((columnIndex + 1), headerRow[columnIndex], "String"));
						}
					}
				}
			}
			return this.Columns;
		}

		public override void SetColumns(IEnumerable<DatabaseTableColumn> columns)
			=> this.Columns = columns
				.Select(c => new DatabaseTableColumn(c.Postion, c.Name, "String"))
				.ToList();

		public override void Truncate()
		{
			if (!File.Exists(this.FilePath))
				throw new FileNotFoundException();
			if (this.HasHeaderRow)
				this.WriteHeader();
			else
				this.ClearFile();
		}

		public override DataTable GetDataTable()
		{
			DataTable returnValue = new();
			if (!File.Exists(this.FilePath))
				throw new FileNotFoundException();
			using TextFieldParser textFieldParser = new(this.FilePath);
			textFieldParser.TextFieldType = FieldType.Delimited;
			textFieldParser.Delimiters = [this.ColumnDelimiter];
			Boolean isFirstRow = true;
			String[]? headerRow;
			while (!textFieldParser.EndOfData)
			{
				if (this.HasHeaderRow && isFirstRow)
				{
					isFirstRow = false;
					headerRow = textFieldParser.ReadFields();
					if (headerRow is not null)
						foreach (String field in headerRow)
							returnValue.Columns.Add(field, typeof(String));
				}
				else if (!this.HasHeaderRow && isFirstRow)
				{
					isFirstRow = false;
					String[]? dataRow = textFieldParser.ReadFields();
					if (dataRow is not null)
					{
						for (Int32 columnIndex = 0; columnIndex < dataRow.Length; columnIndex++)
							returnValue.Columns.Add($"Column_{columnIndex}", typeof(String));
						returnValue.Rows.Add(dataRow);
					}
				}
				else
				{
					String[]? dataRow = textFieldParser.ReadFields();
					if (dataRow is not null)
						returnValue.Rows.Add(dataRow);
				}
			}
			this.Columns = [];
			foreach (DataColumn dataColumn in returnValue.Columns)
				this.Columns.Add(new DatabaseTableColumn(dataColumn.Ordinal, dataColumn.ColumnName, dataColumn.DataType.Name));
			return returnValue;
		}

		public void ClearFile()
		{
			using StreamWriter streamWriter = new(this.FilePath, false);
			streamWriter.Flush();
			streamWriter.Close();
		}

		public void WriteHeader()
		{
			using StreamWriter streamWriter = new(this.FilePath, false);
			if (this.Columns is not null && this.Columns.Count > 0)
			{
				String line = String.Empty;
				foreach (DatabaseTableColumn databaseTableColumn in this.Columns.OrderBy(d => d.Postion))
					line +=
						databaseTableColumn.Name.Contains(this.ColumnDelimiter)
							? $"\"{databaseTableColumn.Name}\"{this.ColumnDelimiter}"
							: $"{databaseTableColumn.Name}{this.ColumnDelimiter}"
					;
				if (line.EndsWith(this.ColumnDelimiter))
					line = line[..^this.ColumnDelimiter.Length];
				streamWriter.WriteLine($"{line}");
			}
			streamWriter.Flush();
			streamWriter.Close();
		}

		public override void WriteDataTable(DataTable dataTable, List<DatabaseTableColumnMapping> mappings)
		{
			List<DatabaseTableColumnMapping> sortedMappings = [.. mappings.OrderBy(m => m.Target.Postion)];
			if (this.HasHeaderRow)
				this.WriteHeader();
			else
				this.ClearFile();
			using StreamWriter streamWriter = new(this.FilePath, true);
			foreach (DataRow dataRow in dataTable.Rows)
			{
				String line = String.Empty;
				foreach (DatabaseTableColumnMapping databaseTableColumnMapping in sortedMappings)
				{
					if (databaseTableColumnMapping.Source is not null)
					{
						String value = dataRow[databaseTableColumnMapping.Source.Name]?.ToString() ?? String.Empty;
						line +=
							value.Contains(this.ColumnDelimiter)
								? $"\"{value}\"{this.ColumnDelimiter}"
								: $"{value}{this.ColumnDelimiter}"
						;
					}
				}
				if (line.EndsWith(this.ColumnDelimiter))
					line = line[..^this.ColumnDelimiter.Length];
				streamWriter.WriteLine($"{line}");
			}
			streamWriter.Flush();
			streamWriter.Close();
		}

		public override void ExecuteQuery(String query, Dictionary<String, Object>? parameters)
			=> throw new NotImplementedException();
		public override String ExecuteScalar(String query, Dictionary<String, Object>? parameters)
			=> throw new NotImplementedException();
	}
}
