using System.Data;

namespace DataMover.Core
{
	public interface IDataLayer
	{
		public String DataLayerType { get; set; }
		public String Description { get; set; }
		public String ConnectionString { get; set; }
        public String SchemaName { get; set; }
		public String TableName { get; set; }
		public String? QualifiedDatabaseName { get; set; }
		public String QualifiedObjectName { get; set; }
		public List<DatabaseTableColumn> GetColumns();
		public void SetColumns(IEnumerable<DatabaseTableColumn> columns);
		public void SetQuailifiedNames(String? qualifiedDatabaseName, String qualifiedObjectName);
		public void Truncate();

		public DataTable GetDataTable();

		public void WriteDataTable(DataTable dataTable, List<DatabaseTableColumnMapping> mappings);

		public void ExecuteQuery(String query, Dictionary<String, Object>? parameters);
		public String ExecuteScalar(String query, Dictionary<String, Object>? parameters);
	}
}
