using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using LethalLevelLoader;

public static class CelestialTintLoader
{
    private static string assetBundleName = "OrbitPrefabBundle";
    private static AssetBundle assetBundle;

    private static Dictionary<string, string> tagPrefabNameMapping = new Dictionary<string, string>
    {
        // Legacy Tags from v1.1.6
        { "Desert", "Prefab_Canyon" },
        { "Forest", "Prefab_Valley" },
        { "Snow", "Prefab_Tundra" },
        
        { "Wasteland", "Prefab_Wasteland" },
        { "Valley", "Prefab_Valley" },
        { "Marsh", "Prefab_Valley" },       // temp prefab
        { "Tundra", "Prefab_Tundra" },
        { "Canyon", "Prefab_Canyon" },
        { "Company", "Prefab_Company" },
        { "Ocean", "Prefab_Ocean" },
        { "Rocky", "Prefab_Rocky" },
        { "Volcanic", "Prefab_Volcanic" },
        { "Argon", "Prefab_Argon" },
    };

    private static Dictionary<string, string> fallbackPrefabNameMapping = new Dictionary<string, string>
    {
        { "moon1", "Prefab_Wasteland" },
        { "moon2", "Prefab_Valley" },
        { "moon3", "Prefab_Tundra" }
    };

    public static void Initialize()
    {
        if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[Celestial Tint Loader] Loader initialized");

        // Initialize Harmony
        Harmony harmony = new Harmony("com.celestialtint.mod");
        harmony.PatchAll(typeof(CelestialTintLoader));

        LoadAssetBundle();
    }

    private static void LoadAssetBundle()
    {
        // Get Assetbundle location
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyDirectory, assetBundleName);

        if (File.Exists(bundlePath))
        {
            assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle != null)
            {
                if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] AssetBundle loaded successfully from path: {bundlePath}");
            }
            else
            {
                if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogError($"[Celestial Tint Loader] Failed to load AssetBundle at path: {bundlePath}");
            }
        }
        else
        {
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogError($"[Celestial Tint Loader] AssetBundle not found at path: {bundlePath}");
        }
    }

    private static void ReplaceCurrentPlanetPrefabs()
    {
        if (assetBundle != null)
        {
            // Loop through all ExtendedLevels
            foreach (ExtendedLevel extendedLevel in LethalLevelLoader.PatchedContent.ExtendedLevels)
            {
                SelectableLevel selectableLevel = extendedLevel.SelectableLevel;
                bool foundCompatiblePrefab = false;

                if (!string.IsNullOrEmpty(CelestialTint.ModConfig.PlanetTagMappings.Value))
                {
                    foreach (string mapping in CelestialTint.ModConfig.PlanetTagMappings.Value.Split(','))
                    {
                        string[] parts = mapping.Trim().Split('@'); // Trim leading and trailing whitespace
                        if (parts.Length != 2)
                        {
                            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] Invalid mapping format: {mapping}. Skipping.");
                            continue; // Skip this mapping if format is invalid
                        }

                        string planetName = parts[0].Trim(); // Trim leading and trailing whitespace
                        string tagName = parts[1].Trim(); // Trim leading and trailing whitespace

                        // Compare planetName with ExtendedLevel.NumberlessPlanetName
                        if (extendedLevel.NumberlessPlanetName.Equals(planetName))
                        {
                            // Use the prefab specified in the mapping
                            if (tagPrefabNameMapping.TryGetValue(tagName, out string prefabName))
                            {
                                LoadAndSetPrefab(selectableLevel, prefabName);
                                foundCompatiblePrefab = true;
                                break;
                            }
                        }
                    }
                }

                if (!foundCompatiblePrefab)
                {
                    // Check if there are ContentTags in the ExtendedLevel
                    foreach (ContentTag contentTag in extendedLevel.ContentTags)
                    {
                        if (tagPrefabNameMapping.TryGetValue(contentTag.contentTagName, out string prefabName))
                        {
                            LoadAndSetPrefab(selectableLevel, prefabName);
                            foundCompatiblePrefab = true;
                            break;
                        }
                    }
                }

                // If no tags are found, look at what is inside the planetPrefab slot
                if (!foundCompatiblePrefab)
                {
                    string fallbackPrefabName;
                    if (fallbackPrefabNameMapping.TryGetValue(selectableLevel.planetPrefab.name.ToLower(), out fallbackPrefabName))
                    {
                        LoadAndSetPrefab(selectableLevel, fallbackPrefabName);
                    }
                    else
                    {
                        LoadAndSetPrefab(selectableLevel, "Prefab_Wasteland");
                        if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] No fallback prefab found for level {selectableLevel.PlanetName}. Using default Prefab_Wasteland.");
                    }
                }
            }

            assetBundle.Unload(false);

            // Call StartOfRound.Instance.ChangePlanet() at the end
            StartOfRound.Instance.ChangePlanet();
        }
        else
        {
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogError($"[Celestial Tint Loader] AssetBundle is not loaded. Unable to replace prefabs.");
        }
    }

    // Set the new prefab in the planetPrefab slot
    private static void LoadAndSetPrefab(SelectableLevel selectableLevel, string prefabName)
    {
        GameObject newPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

        if (newPrefab != null)
        {
            selectableLevel.planetPrefab = newPrefab;
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] Replaced planetPrefab for {selectableLevel.PlanetName} with {prefabName}");
        }
        else
        {
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.LogError($"[Celestial Tint Loader] Prefab not found in AssetBundle: {prefabName}");
        }
    }

    [HarmonyPatch(typeof(Terminal), "Start")]
    [HarmonyPostfix]
    private static void Terminal_Start_Postfix()
    {
        // Replace planet prefabs when Terminal has started
        ReplaceCurrentPlanetPrefabs();
    }
}
