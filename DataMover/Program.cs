// See https://aka.ms/new-console-template for more information
using BDMCommandLine;
using DataMover.Core;
using DataMover.Core.Descriptors;
using System.Reflection;


CommandLine commandLine = new("Help");
PluginDescriptors pluginDescriptors = [];
String pluginDirectoryPath = Path.Combine(AppContext.BaseDirectory, "plugins");
if (!Directory.Exists(pluginDirectoryPath))
{
	Directory.CreateDirectory(pluginDirectoryPath);
}
foreach (String filePath in Directory.GetFiles(pluginDirectoryPath, "*.dll"))
{
	Assembly assembly = Assembly.LoadFile(filePath);
	foreach (Type type in assembly.GetTypes())
	{
		if (typeof(PluginDescriptor).IsAssignableFrom(type))
		{
			if (Activator.CreateInstance(type) is PluginDescriptor pluginDescriptor)
				pluginDescriptors.Add(pluginDescriptor);
		}
		else if (typeof(ICommand).IsAssignableFrom(type))
		{
			if (Activator.CreateInstance(type) is ICommand command)
				CommandLine.Commands.Add(command);
		}
	}
}
CommandLine.Commands.Replace(new DataMoverHelpCommand(pluginDescriptors));

ConsoleText.DefaultForegroundColor = Console.ForegroundColor;
ConsoleText.DefaultBackgroundColor = Console.BackgroundColor;

commandLine.Parse(args);

Console.ResetColor();
Console.ForegroundColor = ConsoleText.DefaultForegroundColor;
Console.BackgroundColor = ConsoleText.DefaultBackgroundColor;
