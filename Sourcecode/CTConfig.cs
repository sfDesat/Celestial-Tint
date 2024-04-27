using BepInEx.Configuration;
using System.Collections.Generic;


public class CTConfig
{
	public ConfigEntry<string> PlanetTagMappings { get; set; }

	public CTConfig(ConfigFile configFile)
	{
		// Adjust the description here
		PlanetTagMappings = configFile.Bind("General", "PlanetTagMappings", "", "Mappings of planet names and tags. Separate each mapping with a comma. Each mapping should be in the format 'PlanetName@TagName'. Example: Experimentation@Wasteland, Vow@Valley. Go to the Celestial Tint github wiki page for previews.");
	}
}
