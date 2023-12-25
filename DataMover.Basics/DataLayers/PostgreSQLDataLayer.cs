using DataMover.Core;
using Microsoft.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace DataMover.Basics.DataLayers
{
    public class PostgreSQLDataLayer : DataLayerBase
    {
        public PostgreSQLDataLayer()
            : base("PostgreSQL", "Provides access to methods for PostgreSQL.")
        { }

		public PostgreSQLDataLayer(String connectionString)
			: base("PostgreSQL", "Provides access to methods for PostgreSQL.", connectionString)
		{
			NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new(connectionString);
			this.QualifiedDatabaseName = $"{npgsqlConnectionStringBuilder.Host}.{npgsqlConnectionStringBuilder.Database}";
		}

		public PostgreSQLDataLayer(String connectionString, String schemaName, String tableName)
			: base("PostgreSQL", "Provides access to methods for PostgreSQL.", connectionString, schemaName, tableName)
		{
			NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new(connectionString);
			this.QualifiedDatabaseName = $"{npgsqlConnectionStringBuilder.Host}.{npgsqlConnectionStringBuilder.Database}";
		}

		public override List<DatabaseTableColumn> GetColumns()
        {
            List<DatabaseTableColumn> returnValue = [];
            using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
            using NpgsqlCommand npgsqlCommand = new()
            {
                Connection = npgsqlConnection,
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = @"
				SELECT
					pg_attribute.attnum AS ordinal_position,
					pg_attribute.attname AS column_name,
					pg_catalog.format_type(pg_attribute.atttypid, pg_attribute.atttypmod) AS data_type
					FROM pg_namespace
					INNER JOIN pg_class ON pg_namespace.oid = pg_class.relnamespace
					INNER JOIN pg_attribute ON pg_class.oid = pg_attribute.attrelid
					WHERE
						pg_namespace.nspname = @SchemaName
						AND pg_class.relname = @TableName
						AND pg_attribute.attnum > 0
					ORDER BY pg_attribute.attnum ASC
			"
            };
            npgsqlCommand.Parameters.AddWithValue("@SchemaName", base.SchemaName);
            npgsqlCommand.Parameters.AddWithValue("@TableName", base.TableName);
            npgsqlConnection.Open();
            using NpgsqlDataReader npgsqlDataReader = npgsqlCommand.ExecuteReader();
            while (npgsqlDataReader.Read())
            {
                returnValue.Add(new DatabaseTableColumn(
                    npgsqlDataReader.GetInt32(0),
                    npgsqlDataReader.GetString(1),
                    npgsqlDataReader.GetString(2)
                ));
            }
            npgsqlDataReader.Close();
            npgsqlConnection.Close();
            return returnValue;
        }

        public override void Truncate()
        {
            using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
            using NpgsqlCommand npgsqlCommand = new()
            {
                Connection = npgsqlConnection,
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $"TRUNCATE \"{base.SchemaName}\".\"{base.TableName}\""
            };
            npgsqlConnection.Open();
            npgsqlCommand.ExecuteNonQuery();
            npgsqlConnection.Close();
        }

        public override DataTable GetDataTable()
        {
            DataTable returnValue = new();
            using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
            using NpgsqlCommand npgsqlCommand = new()
            {
                Connection = npgsqlConnection,
                CommandType = CommandType.Text,
                CommandTimeout = 0,
                CommandText = $"SELECT * FROM \"{base.SchemaName}\".\"{base.TableName}\""
            };
            npgsqlConnection.Open();
            returnValue.Load(npgsqlCommand.ExecuteReader());
            npgsqlConnection.Close();
            return returnValue;
        }

        public override void WriteDataTable(DataTable dataTable, List<DatabaseTableColumnMapping> mappings)
        {
            List<DatabaseTableColumnMapping> sortedMappings = [.. mappings.OrderBy(m => m.Target.Postion)];
            using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
            npgsqlConnection.Open();
            using NpgsqlBinaryImporter npgsqlBinaryImporter = npgsqlConnection.BeginBinaryImport(
                $"COPY \"{base.SchemaName}\".\"{base.TableName}\" (\"{String.Join("\", \"", sortedMappings.Select(m => m.Target.Name).ToArray())}\") FROM STDIN BINARY"
            );
            foreach (DataRow dataRow in dataTable.Rows)
            {
                npgsqlBinaryImporter.StartRow();
                foreach (DatabaseTableColumnMapping mapping in sortedMappings)
                {
#pragma warning disable IDE0038 // Use pattern matching
                    if (mapping.Source is not null)
                    {
                        if (dataRow[mapping.Source.Name] is DBNull)
                            npgsqlBinaryImporter.WriteNull();
                        else
                            switch (mapping.Target.DataType)
                            {
                                case "date":
                                    if (dataRow[mapping.Source.Name] is DateOnly)
                                        npgsqlBinaryImporter.Write((DateOnly)dataRow[mapping.Source.Name], NpgsqlDbType.Date);
                                    else if (dataRow[mapping.Source.Name] is DateTime)
                                        npgsqlBinaryImporter.Write(DateOnly.FromDateTime((DateTime)dataRow[mapping.Source.Name]), NpgsqlDbType.Date);
                                    else if (dataRow[mapping.Source.Name] is DateTimeOffset)
                                        npgsqlBinaryImporter.Write(DateOnly.FromDateTime(((DateTimeOffset)dataRow[mapping.Source.Name]).DateTime), NpgsqlDbType.Date);
                                    else
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    break;
                                case "timestamp without time zone":
                                    if (dataRow[mapping.Source.Name] is DateTime)
                                        npgsqlBinaryImporter.Write((DateTime)dataRow[mapping.Source.Name], NpgsqlDbType.Timestamp);
                                    else
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    break;
                                case "timestamp with time zone":
                                    if (dataRow[mapping.Source.Name] is DateTime)
                                        npgsqlBinaryImporter.Write((DateTime)dataRow[mapping.Source.Name], NpgsqlDbType.TimestampTz);
                                    else
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    break;

                                case "time without time zone":
                                    if (dataRow[mapping.Source.Name] is TimeOnly)
                                        npgsqlBinaryImporter.Write((DateOnly)dataRow[mapping.Source.Name], NpgsqlDbType.Time);
                                    else if (dataRow[mapping.Source.Name] is DateTime)
                                        npgsqlBinaryImporter.Write(TimeOnly.FromDateTime((DateTime)dataRow[mapping.Source.Name]), NpgsqlDbType.Time);
                                    else if (dataRow[mapping.Source.Name] is DateTimeOffset)
                                        npgsqlBinaryImporter.Write(TimeOnly.FromDateTime(((DateTimeOffset)dataRow[mapping.Source.Name]).DateTime), NpgsqlDbType.Time);
                                    else if (dataRow[mapping.Source.Name] is TimeSpan)
                                        npgsqlBinaryImporter.Write(TimeOnly.FromTimeSpan((TimeSpan)dataRow[mapping.Source.Name]), NpgsqlDbType.Time);
                                    else
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    break;
                                case "time with time zone":
                                    if (dataRow[mapping.Source.Name] is DateTimeOffset)
                                        npgsqlBinaryImporter.Write((DateTimeOffset)dataRow[mapping.Source.Name], NpgsqlDbType.TimeTz);
                                    else
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    break;

                                case "text":
                                case "name":
                                case "character varying":
                                case "character":
                                case "citext":
                                case "json":
                                case "xml":
                                    npgsqlBinaryImporter.Write(dataRow[mapping.Source.Name].ToString(), NpgsqlDbType.Text);
                                    break;
                                case "smallint":
                                    npgsqlBinaryImporter.Write(Convert.ToInt16(dataRow[mapping.Source.Name]), NpgsqlDbType.Integer);
                                    break;
                                case "integer":
                                    npgsqlBinaryImporter.Write(Convert.ToInt32(dataRow[mapping.Source.Name]), NpgsqlDbType.Integer);
                                    break;
                                case "bigint":
                                    npgsqlBinaryImporter.Write(Convert.ToInt64(dataRow[mapping.Source.Name]), NpgsqlDbType.Bigint);
                                    break;
                                case "double precision":
                                    npgsqlBinaryImporter.Write(Convert.ToDouble(dataRow[mapping.Source.Name]), NpgsqlDbType.Double);
                                    break;
                                case "boolean":
                                    npgsqlBinaryImporter.Write(Convert.ToBoolean(dataRow[mapping.Source.Name]), NpgsqlDbType.Boolean);
                                    break;
                                case "uuid":
                                    if (dataRow[mapping.Source.Name] is not Guid)
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    npgsqlBinaryImporter.Write(dataRow[mapping.Source.Name], NpgsqlDbType.Uuid);
                                    break;
                                case "oid":
                                    if (dataRow[mapping.Source.Name] is not uint)
                                        throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    npgsqlBinaryImporter.Write((uint)dataRow[mapping.Source.Name], NpgsqlDbType.Oid);
                                    break;
                                default:
                                    throw new DataTypeMismatchedException(mapping.Source.Name, mapping.Source.DataType, mapping.Target.Name, mapping.Target.DataType);
                                    //break;
                            }
                    }
#pragma warning restore IDE0038 // Use pattern matching
                }
            }
            npgsqlBinaryImporter.Complete();
            npgsqlBinaryImporter.Close();
            npgsqlConnection.Close();
        }

		public override void ExecuteQuery(String query, Dictionary<String, Object>? parameters)
		{
			using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
			using NpgsqlCommand npgsqlCommand = new()
			{
				Connection = npgsqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = query
			};
			if (parameters is not null)
				foreach (String parameterKey in parameters.Keys)
					npgsqlCommand.Parameters.AddWithValue(parameterKey, parameters[parameterKey]);
			npgsqlConnection.Open();
			npgsqlCommand.ExecuteNonQuery();
			npgsqlConnection.Close();
		}

		public override String ExecuteScalar(String query, Dictionary<String, Object>? parameters)
		{
			String returnValue = String.Empty;
			using NpgsqlConnection npgsqlConnection = new(base.ConnectionString);
			using NpgsqlCommand npgsqlCommand = new()
			{
				Connection = npgsqlConnection,
				CommandType = CommandType.Text,
				CommandTimeout = 0,
				CommandText = query
			};
			if (parameters is not null)
				foreach (String parameterKey in parameters.Keys)
					npgsqlCommand.Parameters.AddWithValue(parameterKey, parameters[parameterKey]);
			npgsqlConnection.Open();
			Object? result = npgsqlCommand.ExecuteScalar();
			if (result is not null
				&& result is not DBNull
				&& !result.Equals(DBNull.Value)
			)
				returnValue = result.ToString() ?? String.Empty;
			npgsqlConnection.Close();
			return returnValue;
		}
	}
}
