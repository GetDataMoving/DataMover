namespace DataMover.Core
{
	public class DatabaseTableColumn
	{
		public Int32 Postion { get; set; }
		public string Name { get; set; }
		public string DataType { get; set; }

		public DatabaseTableColumn()
		{
			this.Postion = 0;
			this.Name = String.Empty;
			this.DataType = String.Empty;
		}

		public DatabaseTableColumn(Int32 postion, String name, String dataType)
		{
			this.Postion = postion;
			this.Name = name;
			this.DataType = dataType;
		}

		public override String ToString()
			=> $"{this.Name} ({this.Postion}, {this.DataType})";
	}
}
