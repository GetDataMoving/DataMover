using BDMCommandLine;
using DataMover.Basics.Commands;
using DataMover.Core;
using DataMover.Core.Descriptors;
using System.Reflection;

namespace DataMover.Basics
{
	public class Descriptor : PluginDescriptor
	{
		public Descriptor()
		{
			base.Name = "Basics";
			base.Description = "Includes basic plugins for SQL Server, PostgreSQL, and Delimintated Files";
			Assembly? assembly = Assembly.GetAssembly(typeof(Descriptor));
			if (assembly is not null)
				foreach (Type type in assembly.GetTypes())
					if (typeof(DataLayerBase).IsAssignableFrom(type))
					{
						if (Activator.CreateInstance(type) is DataLayerBase dataLayerBase)
							this.DataLayers.Add(new DataLayerDescriptor()
							{
								Type = dataLayerBase.DataLayerType,
								Description = dataLayerBase.Description
							});
					}
					else if (typeof(CommandBase).IsAssignableFrom(type))
					{
						if (Activator.CreateInstance(type) is CommandBase commandBase)
							this.Commands.Add(new CommandDescriptor(commandBase));
					}
		}
	}
}
