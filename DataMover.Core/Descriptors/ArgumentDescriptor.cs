using BDMCommandLine;

namespace DataMover.Core.Descriptors
{
	public class ArgumentDescriptor
	{
		public ArgumentDescriptor() { }

		public ArgumentDescriptor(ICommandArgument argument)
		{
			this.Type = argument.Type;
			this.Name = argument.Name;
			this.Description = argument.Description;
			this.Alias = argument.Alias;
			this.IsRequired = argument.IsRequired;
			if (argument.Options is not null)
				this.Options = argument.Options
					.Select(o => new ArgumentOptionDescriptor(o))
					.ToList();
		}

		public ArgumentType Type { get; set; } = ArgumentType.Simple;
		public String Name { get; set; } = String.Empty;
		public String Description { get; set; } = String.Empty;
		public String Alias { get; set; } = String.Empty;
		public Boolean IsRequired { get; set; } = false;
		public List<ArgumentOptionDescriptor> Options { get; set; } = [];
	}
}
