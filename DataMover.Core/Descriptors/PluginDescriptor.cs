namespace DataMover.Core.Descriptors
{
	public class PluginDescriptor
	{
		public PluginDescriptor() { }
		public String Name { get; set; } = String.Empty;
		public String Description { get; set; } = String.Empty;
		public List<DataLayerDescriptor> DataLayers { get; set; } = [];
		public List<CommandDescriptor> Commands { get; set; } = [];
	}
}
