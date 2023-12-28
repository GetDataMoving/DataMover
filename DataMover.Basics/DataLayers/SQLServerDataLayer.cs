using DataMover.Core;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Xml;

namespace DataMover.Basics.DataLayers
{
    public class SQLServerDataLayer : DataLayerBase
	{
		public SQLServerDataLayer()
			: base("SQLServer", "Provides access to methods for SQL Server.")
		{ }

		public SQLServerDataLayer(String connectionString)
			: base("SQLServer", "Provides access to methods for SQL Server.", connectionString)
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new(connectionString);
			this.QualifiedDatabaseName = $"{sqlConnectionStringBuilder.DataSource}.{sqlConnectionStringBuilder.InitialCatalog}";
		}

		public SQLServerDataLayer(String connectionString, String schemaName, String tableName)
			: base("SQLServer", "Provides access to methods for SQL Server.", connectionString, schemaName, tableName)
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new(connectionString);
			this.QualifiedDatabaseName = $"{sqlConnectionStringBuilder.DataSource}.{sqlConnectionStringBuilder.InitialCatalog}";
		}

		public override List<DatabaseTableColumn> GetColumns()
		{
			List<DatabaseTableColumn> returnValue = [];
			using SqlConnection sqlConnection = new(base.ConnectionString);
			using SqlCommand sqlCommand = new()
			{
				Connection = sqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = @"
				SELECT
					COLUMNPROPERTY([columns].[object_id], [columns].[name], 'Ordinal') AS [OrdinalPosition],
					[columns].[name] AS [ColumnName],
					[types].[name] AS [DataType]
					FROM [sys].[schemas]
						INNER JOIN [sys].[objects] ON [schemas].[schema_id] = [objects].[schema_id]
						INNER JOIN  [sys].[columns] ON [objects].[object_id] = [columns].[object_id]
						INNER JOIN [sys].[types]
							ON [columns].[user_type_id] = [types].[user_type_id]
					WHERE
						[schemas].[name] = @SchemaName
						AND [objects].[name] = @TableName
						AND [objects].[type_desc] IN (N'USER_TABLE', N'VIEW')
					ORDER BY COLUMNPROPERTY([columns].[object_id], [columns].[name], 'Ordinal') ASC
			"
			};
			sqlCommand.Parameters.AddWithValue("@SchemaName", base.SchemaName);
			sqlCommand.Parameters.AddWithValue("@TableName", base.TableName);
			sqlConnection.Open();
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			while (sqlDataReader.Read())
			{
				returnValue.Add(new DatabaseTableColumn(
					sqlDataReader.GetInt32(0),
					sqlDataReader.GetString(1),
					sqlDataReader.GetString(2)
				));
			}
			sqlDataReader.Close();
			sqlConnection.Close();
			return returnValue;
		}

		public override void SetColumns(IEnumerable<DatabaseTableColumn> columns)
		{
			//Do Nothing
		}

		public override void Truncate()
		{
			using SqlConnection sqlConnection = new(base.ConnectionString);
			using SqlCommand sqlCommand = new()
			{
				Connection = sqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = $"TRUNCATE TABLE [{base.SchemaName}].[{base.TableName}]"
			};
			sqlConnection.Open();
			sqlCommand.ExecuteNonQuery();
			sqlConnection.Close();
		}

		public override DataTable GetDataTable()
		{
			DataTable returnValue = new();
			using SqlConnection sqlConnection = new(base.ConnectionString);
			using SqlCommand sqlCommand = new()
			{
				Connection = sqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = $"SELECT * FROM [{base.SchemaName}].[{base.TableName}]"
			};
			sqlConnection.Open();
			returnValue.Load(sqlCommand.ExecuteReader());
			sqlConnection.Close();
			return returnValue;
		}

		public override void WriteDataTable(DataTable dataTable, List<DatabaseTableColumnMapping> mappings)
		{
			List<DatabaseTableColumnMapping> sortedMappings = [.. mappings.OrderBy(m => m.Target.Postion)];
			using SqlConnection sqlConnection = new(base.ConnectionString);
			sqlConnection.Open();
			SqlBulkCopy sqlBulkCopy = new(sqlConnection)
			{
				BulkCopyTimeout = 0,
				EnableStreaming = true,
				BatchSize = 1000,
				DestinationTableName = $"[{base.SchemaName}].[{base.TableName}]"
			};
			foreach (DatabaseTableColumnMapping mapping in sortedMappings)
			{
				if (mapping.Source is not null)
					sqlBulkCopy.ColumnMappings.Add(mapping.Source.Name, $"[{mapping.Target.Name}]");
			}
			sqlBulkCopy.WriteToServer(dataTable);
			sqlBulkCopy.Close();
			sqlConnection.Close();
		}

		public override void ExecuteQuery(String query, Dictionary<String, Object>? parameters)
		{
			using SqlConnection sqlConnection = new(base.ConnectionString);
			using SqlCommand sqlCommand = new()
			{
				Connection = sqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = query
			};
			if (parameters is not null)
				foreach (String parameterKey in parameters.Keys)
				{
					String name = parameterKey.StartsWith('@')
						? parameterKey[1..]
						: parameterKey;
					if (parameters[parameterKey] is XmlElement xmlElement)
					{
						SqlXml sqlXml = new(new XmlNodeReader(xmlElement));
						sqlCommand.Parameters.AddWithValue(name,
							sqlXml.IsNull
								? DBNull.Value
								: sqlXml
						);
					}
					else
						sqlCommand.Parameters.AddWithValue(name, parameters[parameterKey]);
				}
			sqlConnection.Open();
			sqlCommand.ExecuteNonQuery();
			sqlConnection.Close();
		}

		public override String ExecuteScalar(String query, Dictionary<String, Object>? parameters)
		{
			String returnValue = String.Empty;
			using SqlConnection sqlConnection = new(base.ConnectionString);
			using SqlCommand sqlCommand = new()
			{
				Connection = sqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = query
			};
			if (parameters is not null)
				foreach (String parameterKey in parameters.Keys)
				{
					String name = parameterKey.StartsWith('@')
						? parameterKey[1..]
						: parameterKey;
					if (parameters[parameterKey] is XmlElement xmlElement)
					{
						SqlXml sqlXml = new(new XmlNodeReader(xmlElement));
						sqlCommand.Parameters.AddWithValue(name,
							sqlXml.IsNull
								? DBNull.Value
								: sqlXml
						);
					}
					else
						sqlCommand.Parameters.AddWithValue(name, parameters[parameterKey]);
				}
			sqlConnection.Open();
			Object? result = sqlCommand.ExecuteScalar();
			if (result is not null
				&& result is not DBNull
				&& !result.Equals(DBNull.Value)
			)
				returnValue = result.ToString() ?? String.Empty;
			sqlCommand.ExecuteNonQuery();
			sqlConnection.Close();
			return returnValue;
		}
	}
}
