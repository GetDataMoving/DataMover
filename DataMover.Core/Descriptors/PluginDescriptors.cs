using BDMCommandLine;
using Newtonsoft.Json;
using System.Collections;
using System.Runtime.CompilerServices;

namespace DataMover.Core.Descriptors
{
	public class PluginDescriptors : IEnumerable<PluginDescriptor>
	{
		public PluginDescriptors() { }

		private readonly List<PluginDescriptor> _PluginDescriptors = [];

		public IEnumerator<PluginDescriptor> GetEnumerator()
			=> this._PluginDescriptors.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> this._PluginDescriptors.GetEnumerator();

		public PluginDescriptor this[Int32 index] => this._PluginDescriptors[index];
		public PluginDescriptor? this[String name] => this.Get(name);

		public Int32 Count => this._PluginDescriptors.Count;

		public Int32 IndexOf(String name)
			=> this._PluginDescriptors.FindIndex(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

		public Boolean Contains(String name)
			=> this._PluginDescriptors.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

		public PluginDescriptor? Get(String name)
			=> this._PluginDescriptors.FirstOrDefault(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

		public void Add(PluginDescriptor value)
			=> this._PluginDescriptors.Add(value);

		public void AddRange(IEnumerable<PluginDescriptor> values)
			=> this._PluginDescriptors.AddRange(values);

		public Boolean TryGet(String name, out PluginDescriptor? result)
		{
			Boolean returnValue;
			if (this.Contains(name) && this[name] is PluginDescriptor Plugin)
			{
				result = Plugin;
				returnValue = true;
			}
			else
			{
				result = null;
				returnValue = false;
			}
			return returnValue;
		}

		public void Remove(String name)
		{
			if (this.Contains(name) && this[name] is PluginDescriptor Plugin)
				this._PluginDescriptors.Remove(Plugin);
		}
		public void Replace(PluginDescriptor plugin)
		{
			if (this.Contains(plugin.Name))
				this.Remove(plugin.Name);
			this.Add(plugin);
		}

		public ConsoleText[] Describe()
		{
			List<ConsoleText> returnValue = [];
			String indent = "  ";
			if (this._PluginDescriptors.Count.Equals(0))
			{
				returnValue.Add(ConsoleText.BlankLine());
				returnValue.Add(ConsoleText.DarkRed("NO PLUGINS FOUND!"));
				returnValue.Add(ConsoleText.BlankLines(2));
			}
			else
			{
				returnValue.Add(ConsoleText.BlankLine());
				returnValue.Add(ConsoleText.DarkRed("***AVAILABLE PLUGINS************************************************************************"));
				returnValue.Add(ConsoleText.BlankLines(2));
				foreach (PluginDescriptor plugin in this._PluginDescriptors)
				{
					returnValue.Add(ConsoleText.Blue($"{plugin.Name}: {plugin.Description}"));
					returnValue.Add(ConsoleText.BlankLines(2));
					if (plugin.DataLayers.Count.Equals(0))
					{
						returnValue.Add(ConsoleText.Green($"{indent}Data Layers: none"));
						returnValue.Add(ConsoleText.BlankLine());
					}
					else
					{
						returnValue.Add(ConsoleText.Green($"{indent}Data Layers:"));
						returnValue.Add(ConsoleText.BlankLine());
						foreach (DataLayerDescriptor dataLayer in plugin.DataLayers)
						{
							returnValue.Add(ConsoleText.Green($"{indent}{indent}{dataLayer.Type}: {dataLayer.Description}"));
							returnValue.Add(ConsoleText.BlankLine());
						}
					}
					if (plugin.Commands.Count.Equals(0))
					{
						returnValue.Add(ConsoleText.BlankLine());
						returnValue.Add(ConsoleText.Green($"{indent}Commands: none"));
						returnValue.Add(ConsoleText.BlankLine());
					}
					else
					{
						returnValue.Add(ConsoleText.BlankLine());
						returnValue.Add(ConsoleText.Green($"{indent}Commands:"));
						returnValue.Add(ConsoleText.BlankLine());
						foreach (CommandDescriptor command in plugin.Commands)
						{
							returnValue.Add(ConsoleText.Green($"{indent}{indent}{command.Name}: {command.Description}"));
							returnValue.Add(ConsoleText.BlankLine());
							returnValue.Add(ConsoleText.Cyan($"{indent}{indent}{indent}Aliases: {String.Join(", ", command.Aliases)}"));
							returnValue.Add(ConsoleText.BlankLine());
							returnValue.Add(ConsoleText.DarkYellow($"{indent}{indent}{indent}{command.Example}"));
							returnValue.Add(ConsoleText.BlankLine());
						}
					}
				}
				returnValue.Add(ConsoleText.BlankLine());
				returnValue.Add(ConsoleText.DarkRed("********************************************************************************************"));
				returnValue.Add(ConsoleText.BlankLine());
			}
			return [.. returnValue];
		}

		public void SaveToFile(String filePath)
			=> File.WriteAllText(
				filePath,
				JsonConvert.SerializeObject(this));

		public static PluginDescriptors LoadFromFile(String filePath)
		{
			PluginDescriptors returnValue = [];
			if (File.Exists(filePath))
			{
				String json = File.ReadAllText(filePath);
				var temp = JsonConvert.DeserializeObject<List<PluginDescriptor>>(json);
				if (temp is not null)
					returnValue.AddRange(temp);
			}
			return returnValue;
		}
	}
}
