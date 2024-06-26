using BepInEx.Configuration;
using System.Collections.Generic;


public class CTConfig
{
    public ConfigEntry<bool> VanillaMode { get; set; }
    public ConfigEntry<string> PlanetTagMappings { get; set; }
    public ConfigEntry<bool> DisplayShipParts { get; set; }
    public ConfigEntry<bool> ShipDoorAccess { get; set; }
    public ConfigEntry<bool> DebugLogging { get; set; }

    public CTConfig(ConfigFile configFile)
	{
        VanillaMode = configFile.Bind("1. Planets", "VanillaMode", false, "Whether the outside should show the vanilla planets. Also adds in a sun object. Illusion can break when enabling ShipDoorAccess.");
        PlanetTagMappings = configFile.Bind("1. Planets", "PlanetTagMappings", "", "Mappings of planet names and tags. Separate each mapping with a comma. Each mapping should be in the format 'PlanetName@TagName'. Example: Experimentation@Wasteland, Vow@Valley. Go to the Celestial Tint github wiki page for previews.");
        DisplayShipParts = configFile.Bind("2. Ship", "DisplayShipParts", false, " Whether to display ship parts in orbit. Only visible if you go outside.");
        ShipDoorAccess = configFile.Bind("2. Ship", "ShipDoorAccess", false, "Whether to door can be opened while in orbit.");
        DebugLogging = configFile.Bind("3. Debugging", "DebugLogging", false, "Whether to display debug messages. When false, Celestial Tint will not display any messages.");
    }
}
