using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine.SceneManagement;

public static class CelestialTintLoader
{
    private static bool hasLoadedAssetbundle = false;

    private static List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private static GameObject prefab;

    private static string assetBundleName = "OrbitPrefabBundle";
    private static string SmallAssetBundleName = "SmallOrbitPrefabBundle";
    private static AssetBundle assetBundle;
    private static AssetBundle SmallAssetBundle;

    private static Dictionary<string, string> tagVanillaNameMapping = new Dictionary<string, string>
    {
        { "Experimentation", "Prefab_Wasteland" },
        { "Vow", "Prefab_Valley" },
        { "Gordion", "Prefab_Company" },
        { "March", "Prefab_Valley" },
        { "Adamance", "Prefab_Valley" },
        { "Rend", "Prefab_Tundra" },
        { "Dine", "Prefab_Tundra" },
        { "Offense", "Prefab_Canyon" },
        { "Assurance", "Prefab_Canyon" },
        { "Titan", "Prefab_Tundra" },
        { "Artifice", "Prefab_Canyon" },
        { "Liquidation", "Prefab_Ocean" },
        { "Embrion", "Prefab_Valley" },
    };

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
        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[Celestial Tint Loader] Loader initialized");

        // Initialize Harmony
        Harmony harmony = new Harmony("com.celestialtint.mod");
        harmony.PatchAll(typeof(CelestialTintLoader));

        LoadAssetBundle();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void LoadAssetBundle()
    {
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyDirectory, assetBundleName);

        string SmallAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string SmallBundlePath = Path.Combine(SmallAssemblyDirectory, SmallAssetBundleName);

        if (File.Exists(bundlePath))
        {
            assetBundle = AssetBundle.LoadFromFile(bundlePath);
            hasLoadedAssetbundle = true;

            if (assetBundle != null)
            {
                if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] AssetBundle loaded successfully from path: {bundlePath}");
            }
            else
            {
                Debug.LogError($"[Celestial Tint Loader] Failed to load AssetBundle at path: {bundlePath}");
            }
        }
        else
        {
            Debug.LogError($"[Celestial Tint Loader] AssetBundle not found at path: {bundlePath}");
        }

        if (File.Exists(SmallBundlePath))
        {
            SmallAssetBundle = AssetBundle.LoadFromFile(SmallBundlePath);

            if (SmallAssetBundle != null)
            {
                if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] AssetBundle loaded successfully from path: {SmallBundlePath}");
            }
            else
            {
                Debug.LogError($"[Celestial Tint Loader] Failed to load AssetBundle at path: {SmallBundlePath}");
            }
        }
        else
        {
            Debug.LogError($"[Celestial Tint Loader] AssetBundle not found at path: {SmallBundlePath}");
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isInOrbit = CelestialTintStart.CheckSceneState();

        if (isInOrbit) LoadStarmapContainerPrefab();
        else UnloadStarmapContainerPrefab();
    }

    private static async void OnSceneUnloaded(Scene scene)
    {
        bool isInOrbit = await CelestialTintStart.DelayedCheckSceneStateAsync();

        if (isInOrbit) LoadStarmapContainerPrefab();
        else UnloadStarmapContainerPrefab();
    }

    private static void LoadStarmapContainerPrefab()
    {
        if (SmallAssetBundle != null)
        {
            prefab = SmallAssetBundle.LoadAsset<GameObject>("StarmapContainer");
            if (prefab != null)
            {
                GameObject instantiatedPrefab = GameObject.Instantiate(prefab, prefab.transform.position, Quaternion.identity);
                instantiatedPrefabs.Add(instantiatedPrefab);

                if (CelestialTintStart.ModConfig.DebugLogging.Value)
                    Debug.Log("[Celestial Tint Loader] Instantiated StarmapContainer prefab.");
            }
            else
            {
                Debug.LogError("[Celestial Tint Loader] StarmapContainer prefab not found in AssetBundle.");
            }
        }
        else
        {
            Debug.LogError("[Celestial Tint Loader] AssetBundle is not loaded.");
        }
    }

    private static void UnloadStarmapContainerPrefab()
    {
        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            GameObject.Destroy(instantiatedPrefab);
        }
        instantiatedPrefabs.Clear();

        if (CelestialTintStart.ModConfig.DebugLogging.Value) 
            Debug.Log("[Celestial Tint Loader] All instantiated StarmapContainer prefabs have been destroyed.");
    }

    private static void ReplaceVanillaPlanetPrefabs()
    {
        if (assetBundle != null)
        {
            StartOfRound startOfRound = GameObject.FindObjectOfType<StartOfRound>();

            foreach (SelectableLevel selectableLevel in startOfRound.levels)
            {
                bool foundCompatiblePrefab = false;

                // Check if there are custom mappings in the config file
                if (!string.IsNullOrEmpty(CelestialTintStart.ModConfig.PlanetTagMappings.Value))
                {
                    foreach (string mapping in CelestialTintStart.ModConfig.PlanetTagMappings.Value.Split(','))
                    {
                        string[] parts = mapping.Trim().Split('@'); // Trim leading and trailing whitespace
                        if (parts.Length != 2)
                        {
                            if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] Invalid mapping format: {mapping}. Skipping.");
                            continue;
                        }

                        string planetNameConfig = parts[0].Trim(); // Trim leading and trailing whitespace
                        string tagName = parts[1].Trim(); // Trim leading and trailing whitespace

                        string planetName = selectableLevel.PlanetName.TrimStart(' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                        // Compare planetName with selectableLevel.PlanetName
                        if (planetName.Equals(planetNameConfig, StringComparison.OrdinalIgnoreCase))
                        {
                            // Use the prefab specified in the mapping
                            if (tagPrefabNameMapping.TryGetValue(tagName, out string prefabName))
                            {
                                LoadAndSetPrefab(selectableLevel, prefabName);
                                foundCompatiblePrefab = true;
                                if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] Replaced planetPrefab for {selectableLevel.PlanetName} with {prefabName} based on custom mapping.");
                                break;
                            }
                        }
                    }
                }

                // If no custom mappings are found, use the default vanilla name mapping
                if (!foundCompatiblePrefab)
                {
                    string planetName = selectableLevel.PlanetName.TrimStart(' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                    string prefabName;

                    if (tagVanillaNameMapping.TryGetValue(planetName, out prefabName))
                    {
                        LoadAndSetPrefab(selectableLevel, prefabName);
                    }
                    else
                    {
                        LoadAndSetPrefab(selectableLevel, "Prefab_Wasteland");
                        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] No fallback prefab found for level {selectableLevel.PlanetName}. Using default Prefab_Wasteland.");
                    }
                }
            }

            // Call StartOfRound.Instance.ChangePlanet() at the end
            StartOfRound.Instance.ChangePlanet();
        }
        else
        {
            if (hasLoadedAssetbundle == false) Debug.LogError($"[Celestial Tint Loader] AssetBundle is not loaded. Unable to replace prefabs.");
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

                // Checking config file entries
                if (!string.IsNullOrEmpty(CelestialTintStart.ModConfig.PlanetTagMappings.Value))
                {
                    foreach (string mapping in CelestialTintStart.ModConfig.PlanetTagMappings.Value.Split(','))
                    {
                        string[] parts = mapping.Trim().Split('@'); // Trim leading and trailing whitespace
                        if (parts.Length != 2)
                        {
                            if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] Invalid mapping format: {mapping}. Skipping.");
                            continue;
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
                        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.LogWarning($"[Celestial Tint Loader] No fallback prefab found for level {selectableLevel.PlanetName}. Using default Prefab_Wasteland.");
                    }
                }
            }

            // Call StartOfRound.Instance.ChangePlanet() at the end
            StartOfRound.Instance.ChangePlanet();
        }
        else
        {
            if (hasLoadedAssetbundle == false) Debug.LogError($"[Celestial Tint Loader] AssetBundle is not loaded. Unable to replace prefabs.");
        }
    }

    // Set the new prefab in the planetPrefab slot
    private static void LoadAndSetPrefab(SelectableLevel selectableLevel, string prefabName)
    {
        GameObject newPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

        if (newPrefab != null)
        {
            selectableLevel.planetPrefab = newPrefab;
            if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log($"[Celestial Tint Loader] Replaced planetPrefab for {selectableLevel.PlanetName} with {prefabName}");
        }
        else
        {
            Debug.LogError($"[Celestial Tint Loader] Prefab not found in AssetBundle: {prefabName}");
        }
    }

    [HarmonyPatch(typeof(Terminal), "Start")]
    [HarmonyPostfix]
    private static void Terminal_Start_Postfix()
    {
        // Get a list of all loaded assemblies
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Check if the assembly of the other mod is loaded
        bool isOtherModLoaded = loadedAssemblies.Any(assembly => assembly.FullName.StartsWith("LethalLevelLoader"));

        if (isOtherModLoaded)
        {
            // The other mod is loaded, do something
            if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[Celestial Tint Loader] Detected LethalLevelLoader");

            ReplaceCurrentPlanetPrefabs();
        }
        else
        {
            // The other mod is not loaded
            if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[Celestial Tint Loader] Did not detect LethalLevelLoader");

            ReplaceVanillaPlanetPrefabs();
        }

        LoadStarmapContainerPrefab();

        if (assetBundle != null) assetBundle.Unload(false);
    }
}
