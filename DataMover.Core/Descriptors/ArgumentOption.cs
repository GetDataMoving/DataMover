using BDMCommandLine;

namespace DataMover.Core.Descriptors
{
	public class ArgumentOptionDescriptor(ICommandArgumentOption option)
	{
		public String Value { get; set; } = option.Value;
		public String Description { get; set; } = option.Description;
	}
}
