using BDMCommandLine;
using DataMover.Basics.Commands;
using DataMover.Core;
using DataMover.Core.Descriptors;
using System.Diagnostics;
using System.Reflection;

namespace DataMover.Basics
{
	public class Descriptor : PluginDescriptor
	{
		public Descriptor()
		{
			Assembly? assembly = Assembly.GetAssembly(typeof(Descriptor));
			AssetVersion assetVersion = PluginDescriptor.GetAssetVersion(
				assembly,
				"DataMover.Basics",
				"v1.0.0",
				"Includes basic plugins for SQL Server, PostgreSQL, and Delimintated Files",
				"Copyright © 2023 GetDataMoving.org",
				$"https://getdatamoving.org/plugins/DataMover.Basics"
			);
			base.Name = assetVersion.Name;
			base.Version = assetVersion.Version;
			base.Description = assetVersion.Description;
			base.Copyright = assetVersion.Copyright;
			base.InfoURL = $"https://getdatamoving.org/plugins/{base.Name}/{base.Version}";
			if (assembly is not null)
			{
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
}
