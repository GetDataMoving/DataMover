using System.Data;

namespace DataMover.Core
{
    public class DataLayerBase : IDataLayer
    {
        public String DataLayerType { get; set; }
		public String Description { get; set; }
		public String ConnectionString { get; set; }
        public String SchemaName { get; set; }
        public String TableName { get; set; }
        public String? QualifiedDatabaseName { get; set; }
        public virtual String QualifiedObjectName { get; set; }

        public DataLayerBase()
        {
            this.DataLayerType = "Undefined";
			this.Description = "Undefined";
			this.ConnectionString = String.Empty;
			this.SchemaName = String.Empty;
			this.TableName = String.Empty;
			this.QualifiedObjectName = this.QualifiedDatabaseName is not null
				? $"{this.QualifiedDatabaseName}.{this.SchemaName}.{this.TableName}"
				: $"{this.SchemaName}.{this.TableName}";
		}
		public DataLayerBase(String dataLayerType, String description)
		{
			this.DataLayerType = dataLayerType;
			this.Description = description;
			this.ConnectionString = String.Empty;
			this.SchemaName = String.Empty;
			this.TableName = String.Empty;
			this.QualifiedObjectName = this.QualifiedDatabaseName is not null
				? $"{this.QualifiedDatabaseName}.{this.SchemaName}.{this.TableName}"
				: $"{this.SchemaName}.{this.TableName}";
		}

		public DataLayerBase(String dataLayerType, String description, String connectionString)
            : this(dataLayerType, description)
        {
			this.ConnectionString = connectionString;
			this.SchemaName = String.Empty;
			this.TableName = String.Empty;
			this.QualifiedObjectName = this.QualifiedDatabaseName is not null
				? $"{this.QualifiedDatabaseName}.{this.SchemaName}.{this.TableName}"
				: $"{this.SchemaName}.{this.TableName}";
		}

		public DataLayerBase(String dataLayerType, String description, String connectionString, String schemaName, String tableName)
			: this(dataLayerType, description, connectionString)
		{
			this.SchemaName = schemaName;
			this.TableName = tableName;
			this.QualifiedObjectName = this.QualifiedDatabaseName is not null
				? $"{this.QualifiedDatabaseName}.{this.SchemaName}.{this.TableName}"
				: $"{this.SchemaName}.{this.TableName}";
		}

		public virtual void SetQuailifiedNames(String? qualifiedDatabaseName, String qualifiedObjectName)
        {
			this.QualifiedDatabaseName = qualifiedDatabaseName;
			this.QualifiedObjectName = qualifiedObjectName;
        }

        public virtual List<DatabaseTableColumn> GetColumns()
            => throw new NotImplementedException();

		public virtual void SetColumns(IEnumerable<DatabaseTableColumn> columns)
            => throw new NotImplementedException();

		public virtual void Truncate()
            => throw new NotImplementedException();

        public virtual DataTable GetDataTable()
            => throw new NotImplementedException();

        public virtual void WriteDataTable(DataTable dataTable, List<DatabaseTableColumnMapping> mappings)
            => throw new NotImplementedException();

        public virtual void ExecuteQuery(String query, Dictionary<String, object>? parameters)
            => throw new NotImplementedException();

		public virtual String ExecuteScalar(String query, Dictionary<String, Object>? parameters)
			=> throw new NotImplementedException();
	}
}
