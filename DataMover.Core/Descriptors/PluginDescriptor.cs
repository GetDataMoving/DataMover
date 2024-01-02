using BDMCommandLine;
using System.Diagnostics;
using System.Reflection;

namespace DataMover.Core.Descriptors
{
	public class PluginDescriptor
	{
		public PluginDescriptor() { }
		public String Name { get; set; } = String.Empty;
		public String Description { get; set; } = String.Empty;
		public String Version { get; set; } = String.Empty;
		public String Copyright { get; set; } = String.Empty;
		public String InfoURL { get; set; } = String.Empty;
		public List<DataLayerDescriptor> DataLayers { get; set; } = [];
		public List<CommandDescriptor> Commands { get; set; } = [];

		public AssetVersion GetAssetVersion()
			=> new(this.Name, this.Version, this.Description, this.Copyright, this.InfoURL);

		public static AssetVersion GetAssetVersion(
			Assembly? assembly,
			String defaultName,
			String defaultVersion,
			String defaultDescription,
			String defaultCopyright,
			String infoURL)
		{
			AssetVersion returnValue = new();
			FileVersionInfo? fileVersionInfo = null;
			String? fileVersionInfoProductName = null;
			String? fileVersionInfoVersion = null;
			String? fileVersionInfoDescription = null;
			String? fileVersionInfoCopyright = null;
			String? assemblyName = null;
			String? assemblyVersion = null;
			if (assembly is not null)
			{
				if (File.Exists(assembly.Location))
					fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
				if (fileVersionInfo is not null)
				{
					fileVersionInfoProductName = fileVersionInfo.ProductName;
					if (!String.IsNullOrEmpty(fileVersionInfo.ProductVersion))
						fileVersionInfoVersion = $"v{fileVersionInfo.ProductMajorPart}.{fileVersionInfo.ProductMinorPart}.{fileVersionInfo.ProductBuildPart}";
					fileVersionInfoDescription = fileVersionInfo.Comments;
					fileVersionInfoCopyright = fileVersionInfo.LegalCopyright;
				}
				AssemblyName assemblyNameObject = assembly.GetName();
				assemblyName = assemblyNameObject.Name;
				Version? version = assemblyNameObject.Version;
				if (version is not null)
					assemblyVersion = $"v{version.Major}.{version.Minor}.{version.Build}";
			}
			returnValue.Name = assemblyName ?? fileVersionInfoProductName ?? defaultName;
			returnValue.Version = assemblyVersion ?? fileVersionInfoVersion ?? defaultVersion;
			returnValue.Description = fileVersionInfoDescription ?? defaultDescription;
			returnValue.Copyright = fileVersionInfoCopyright ?? defaultCopyright;
			returnValue.InfoURL = infoURL;
			return returnValue;
		}
	}
}
