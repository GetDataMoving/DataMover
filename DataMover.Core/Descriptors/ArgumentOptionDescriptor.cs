using BDMCommandLine;

namespace DataMover.Core.Descriptors
{
	public class ArgumentOptionDescriptor
	{
		public ArgumentOptionDescriptor() { }

		public ArgumentOptionDescriptor(ICommandArgumentOption option)
		{
			this.Value = option.Value;
			this.Description = option.Description;
		}
		
		public String Value { get; set; } = String.Empty;
		public String Description { get; set; } = String.Empty;
	}
}
