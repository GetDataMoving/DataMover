// See https://aka.ms/new-console-template for more information
using BDMCommandLine;
using DataMover.Core;
using DataMover.Core.Descriptors;
using System.Diagnostics;
using System.Reflection;


CommandLine commandLine = new("Help");
CommandLine.AssetVersions.Clear();

Assembly executingAssembly = Assembly.GetExecutingAssembly();
if (executingAssembly is not null)
{
	CommandLine.AssetVersions.Replace(PluginDescriptor.GetAssetVersion(
		executingAssembly,
		"DataMover", "v0.0.0", "Moves data between various database systems", "Copyright © 2023 GetDataMoving.org",
		"https://getdatamoving.org/DataMover"
	));
}

PluginDescriptors pluginDescriptors = [];
String pluginDirectoryPath = Path.Combine(AppContext.BaseDirectory, "plugins");
if (!Directory.Exists(pluginDirectoryPath))
{
	Directory.CreateDirectory(pluginDirectoryPath);
}
foreach (String filePath in Directory.GetFiles(pluginDirectoryPath, "*.dll"))
{
	Assembly assembly = Assembly.LoadFile(filePath);
	if (assembly.GetName().Name == "DataMover")
	{
		CommandLine.AssetVersions.Replace(PluginDescriptor.GetAssetVersion(
			assembly,
			"DataMover", "v0.0.0", "Moves data between various database systems", "Copyright © 2023 GetDataMoving.org",
			"https://getdatamoving.org/DataMover"
		));
	}
	else if (assembly.GetName().Name == "DataMover.Core")
	{
		CommandLine.AssetVersions.Replace(PluginDescriptor.GetAssetVersion(
			assembly,
			"DataMover.Core", "v0.0.0", "Underlying system for DataMover App", "Copyright © 2023 GetDataMoving.org",
			"https://getdatamoving.org/DataMover.Core"
		));
	}
	foreach (Type type in assembly.GetTypes())
	{
		String fullName = type.FullName ?? type.Name;
		if (
			fullName.StartsWith("DataMover")
			&& !fullName.Equals("DataMover.Core.DataMoverCommandBase")
			&& !fullName.Equals("DataMover.Core.DataCopyCommandBase")
			&& !fullName.Equals("DataMover.Core.QueryExecuteCommandBase")
			&& !fullName.Equals("DataMover.Core.QueryExecuteScalarCommandBase")
			&& !fullName.Equals("DataMover.Core.Descriptors.PluginDescriptor")
		)
		{
			if (typeof(PluginDescriptor).IsAssignableFrom(type))
			{
				var pluginDescriptor = Activator.CreateInstance(type) as PluginDescriptor;
				if (pluginDescriptor is not null)
				{
					pluginDescriptors.Add(pluginDescriptor);
					CommandLine.AssetVersions.Replace(pluginDescriptor.GetAssetVersion());
				}
			}
			else if (typeof(ICommand).IsAssignableFrom(type)
			)
			{
				var command = Activator.CreateInstance(type) as ICommand;
				if (command is not null)
					CommandLine.Commands.Add(command);
			}
		}
	}
}
pluginDescriptors.SaveToFile(Path.Combine(AppContext.BaseDirectory, "plugins", "plugins.json"));
CommandLine.Commands.Replace(new DataMoverHelpCommand());

ConsoleText.DefaultForegroundColor = Console.ForegroundColor;
ConsoleText.DefaultBackgroundColor = Console.BackgroundColor;

commandLine.Parse(args);

Console.ResetColor();
Console.ForegroundColor = ConsoleText.DefaultForegroundColor;
Console.BackgroundColor = ConsoleText.DefaultBackgroundColor;
