using BDMCommandLine;

namespace DataMover.Core.Descriptors
{
	public class CommandDescriptor
	{
		public CommandDescriptor() { }
		public CommandDescriptor(ICommand command)
		{
			this.Name  = command.Name;
			this.Description = command.Description;
			this.Usage = command.Usage;
			this.Example = command.Example;
			this.Aliases = [.. command.Aliases];
			this.Arguments = command.Arguments.Select(a => new ArgumentDescriptor(a)).ToList();
		}
		public String Name { get; set; } = String.Empty;
		public String Description { get; set; } = String.Empty;
		public String Usage { get; set; } = String.Empty;
		public String Example { get; set; } = String.Empty;
		public List<String> Aliases { get; set; } = [];
		public List<ArgumentDescriptor> Arguments { get; set; } = [];
	}
}
