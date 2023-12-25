using BDMCommandLine;

namespace DataMover.Core.Descriptors
{
	public class CommandDescriptor(ICommand command)
	{
		public String Name { get; set; } = command.Name;
		public String Description { get; set; } = command.Description;
		public String Usage { get; set; } = command.Usage;
		public String Example { get; set; } = command.Example;
		public List<String> Aliases { get; set; } = [.. command.Aliases];
		public List<ArgumentDescriptor> Arguments { get; set; } = command.Arguments.Select(a => new ArgumentDescriptor(a)).ToList();

	}
}
