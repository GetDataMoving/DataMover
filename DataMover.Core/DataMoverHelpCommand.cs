using BDMCommandLine;
using DataMover.Core.Descriptors;
using System.Collections.Generic;

namespace DataMover.Core
{
	public class DataMoverHelpCommand : CommandBase
	{
		public PluginDescriptors? PluginDescriptors { get; set; }

		public DataMoverHelpCommand()
			: base(
				HelpCommand.CommandName,
				"Used to display help.",
				"Help [command]",
				"{EXEPath} help [command]",
				["help"],
				new CommandArgumentBase[] {
						CommandArgumentBase.CreateSimpleArgument("Command", "c", "Command to get help on.", false)
				}
		) { }

		public DataMoverHelpCommand(PluginDescriptors pluginDescriptors)
			: base(
				HelpCommand.CommandName,
				"Used to display help.",
				"Help [command]",
				"{EXEPath} help [command]",
				["help"],
				new CommandArgumentBase[] {
						CommandArgumentBase.CreateSimpleArgument("Command", "c", "Command to get help on.", false)
				}
		)
		{
			this.PluginDescriptors = pluginDescriptors;
		}

		public override ConsoleText[] GetHelpText()
		{
			List<ConsoleText> returnValue = [];
			if (
				this.PluginDescriptors is null
				|| this.PluginDescriptors.Count < 0)
				this.PluginDescriptors = PluginDescriptors.LoadFromFile(Path.Combine(AppContext.BaseDirectory, "plugins", "plugins.json"));
			this.PluginDescriptors ??= [];
			returnValue.AddRange([
				ConsoleText.Red("To view help for a specific command use:"),
				ConsoleText.BlankLines(2),
				ConsoleText.DarkGreen("  DataMover help {command name or alias}"),
				ConsoleText.BlankLines(2)
			]);
			returnValue.AddRange(this.PluginDescriptors.Describe());
			return [..returnValue];
		}

		public override void Execute()
        {
			if (CommandLine.Commands.TryGet(base.Arguments.GetSimpleValue("Command"), out ICommand? command)
				&& command is not null
			)
				CommandLine.OutputTextCollection(command.GetHelpText());
			else
				CommandLine.OutputTextCollection(this.GetHelpText());
        }
    }
}
