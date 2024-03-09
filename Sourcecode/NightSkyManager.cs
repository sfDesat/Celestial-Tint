using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using LethalLevelLoader;

public static class NightSkyManager
{
	private static string assetBundleName = "OrbitPrefabBundle";
	private static AssetBundle assetBundle;

	private static Dictionary<string, string> tagPrefabNameMapping = new Dictionary<string, string>
	{
		{ "Wasteland", "Prefab_Wasteland" },
		{ "Forest", "Prefab_Forest" },
		{ "Snow", "Prefab_Snow" },
		{ "Desert", "Prefab_Desert" },
		{ "Company", "Prefab_Company" },
		{ "Ocean", "Prefab_Ocean" }
	};

	private static Dictionary<string, string> fallbackPrefabNameMapping = new Dictionary<string, string>
	{
		{ "moon1", "Prefab_Wasteland" },
		{ "moon2", "Prefab_Forest" },
		{ "moon3", "Prefab_Snow" }
	};

	public static void Initialize()
	{
		Debug.Log("[NightSkyPlugin] Nightsky manager initialized");

		// Initialize Harmony
		Harmony harmony = new Harmony("com.nightsky.mod");
		harmony.PatchAll(typeof(NightSkyManager));

		LoadAssetBundle();
	}

	private static void LoadAssetBundle()
	{
		string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		string bundlePath = Path.Combine(assemblyDirectory, assetBundleName);

		if (File.Exists(bundlePath))
		{
			assetBundle = AssetBundle.LoadFromFile(bundlePath);

			if (assetBundle != null)
			{
				Debug.Log($"[NightSkyPlugin] AssetBundle loaded successfully from path: {bundlePath}");
			}
			else
			{
				Debug.LogError($"[NightSkyPlugin] Failed to load AssetBundle at path: {bundlePath}");
			}
		}
		else
		{
			Debug.LogError($"[NightSkyPlugin] AssetBundle not found at path: {bundlePath}");
		}
	}

	private static void ReplaceCurrentPlanetPrefabs()
	{
		if (assetBundle != null)
		{
			ExtendedLevel[] extendedLevels = Resources.FindObjectsOfTypeAll<ExtendedLevel>();

			foreach (ExtendedLevel extendedLevel in extendedLevels)
			{
				SelectableLevel selectableLevel = extendedLevel.selectableLevel;
				bool foundCompatiblePrefab = false;

				foreach (string levelTag in extendedLevel.levelTags)
				{
					if (tagPrefabNameMapping.TryGetValue(levelTag, out string compatibleTag))
					{
						string prefabName = tagPrefabNameMapping[levelTag];
						LoadAndSetPrefab(selectableLevel, prefabName);
						foundCompatiblePrefab = true;
						break;
					}
				}

				if (!foundCompatiblePrefab)
				{
					string fallbackPrefabName;
					if (fallbackPrefabNameMapping.TryGetValue(selectableLevel.PlanetName.ToLower(), out fallbackPrefabName))
					{
						LoadAndSetPrefab(selectableLevel, fallbackPrefabName);
					}
					else
					{
						LoadAndSetPrefab(selectableLevel, "Prefab_Wasteland");
						Debug.LogWarning($"[NightSkyPlugin] No fallback prefab found for level {selectableLevel.PlanetName}. Using default Prefab_Wasteland.");
					}
				}
			}

			assetBundle.Unload(false);

			// Call StartOfRound.Instance.ChangePlanet() at the end
			StartOfRound.Instance.ChangePlanet();
		}
		else
		{
			Debug.LogError($"[NightSkyPlugin] AssetBundle is not loaded. Unable to replace prefabs.");
		}
	}

	private static void LoadAndSetPrefab(SelectableLevel selectableLevel, string prefabName)
	{
		GameObject newPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

		if (newPrefab != null)
		{
			selectableLevel.planetPrefab = newPrefab;
			Debug.Log($"[NightSkyPlugin] Replaced planetPrefab for {selectableLevel.PlanetName} with {prefabName}");
		}
		else
		{
			Debug.LogError($"[NightSkyPlugin] Prefab not found in AssetBundle: {prefabName}");
		}
	}

	[HarmonyPatch(typeof(Terminal), "Start")]
	[HarmonyPostfix]
	private static void Terminal_Start_Postfix()
	{
		ReplaceCurrentPlanetPrefabs();
	}
}
