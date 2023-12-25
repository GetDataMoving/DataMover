using BDMCommandLine;
using DataMover.Core.Descriptors;
using System.Collections.Generic;

namespace DataMover.Core
{
	public class DataMoverHelpCommand(PluginDescriptors pluginDescriptors) : CommandBase(
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
		private readonly PluginDescriptors _PluginDescriptors = pluginDescriptors;

		public override ConsoleText[] GetHelpText()
		{
			List<ConsoleText> returnValue = [];
			returnValue.AddRange([
				ConsoleText.Red("To view help for a specific command use:"),
				ConsoleText.BlankLines(2),
				ConsoleText.DarkGreen("  DataMover help {command name or alias}"),
				ConsoleText.BlankLines(2)
			]);
			returnValue.AddRange(this._PluginDescriptors.Describe());
			return [.. returnValue];
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
