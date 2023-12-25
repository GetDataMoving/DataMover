namespace DataMover.Core
{
	public class DatabaseTableColumnMapping(DatabaseTableColumn target, DatabaseTableColumn? source)
	{
		public DatabaseTableColumn Target { get; set; } = target;
		public DatabaseTableColumn? Source { get; set; } = source;

		public DatabaseTableColumnMapping(DatabaseTableColumn target)
			: this (target, null)
		{ }
		public override String ToString()
			=> $"{this.Target.Name} ({this.Target.Postion}, {this.Target.DataType})";
	}
}
