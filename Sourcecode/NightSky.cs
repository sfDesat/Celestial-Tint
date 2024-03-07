using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

[BepInPlugin("NightSkyPlugin", "Night Sky Plugin", "1.0.1")]
public class NightSkyPlugin : BaseUnityPlugin
{
    private string assetBundleName = "OrbitPrefabBundle";
    private AssetBundle assetBundle;

    // Dictionary to map moon names to prefab names
    private Dictionary<string, string> prefabNameMapping = new Dictionary<string, string>
    {
        { "moon1", "Prefab1" },
        { "moon2", "Prefab2" },
        { "moon3", "Prefab3" }
        // Add more mappings as needed
    };

    void Awake()
    {
        Debug.Log("[NightSkyPlugin] Nightsky loaded");
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadAssetBundle();
    }

    private void LoadAssetBundle()
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[NightSkyPlugin] Scene loaded: {scene.name}, Level ID: {scene.buildIndex}");

        if (scene.name == "SampleSceneRelay")
        {
            Debug.Log("[NightSkyPlugin] SampleSceneRelay loaded");
            ReplaceCurrentPlanetPrefabs();
        }
    }

    private void ReplaceCurrentPlanetPrefabs()
    {
        if (assetBundle != null)
        {
            SelectableLevel[] selectableLevels = Resources.FindObjectsOfTypeAll<SelectableLevel>();

            foreach (SelectableLevel selectableLevel in selectableLevels)
            {
                string moonName = selectableLevel.planetPrefab.name.ToLower();

                // Check if the moonName is in the mapping
                if (prefabNameMapping.TryGetValue(moonName, out string prefabName))
                {
                    GameObject newPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

                    if (newPrefab != null)
                    {
                        selectableLevel.planetPrefab = newPrefab;
                        Debug.Log($"[NightSkyPlugin] Replaced currentPlanetPrefab for {moonName} with {prefabName}");
                    }
                    else
                    {
                        Debug.LogError($"[NightSkyPlugin] Prefab not found for {moonName} in AssetBundle");
                    }
                }
                else
                {
                    Debug.LogError($"[NightSkyPlugin] Unknown moon name: {moonName}");
                }
            }

            assetBundle.Unload(false);
        }
        else
        {
            Debug.LogError($"[NightSkyPlugin] AssetBundle is not loaded. Unable to replace prefabs.");
        }
    }
}
