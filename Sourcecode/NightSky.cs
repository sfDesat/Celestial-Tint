using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;

[BepInPlugin("NightSkyPlugin", "Night Sky Plugin", "1.0.1")]
public class NightSkyPlugin : BaseUnityPlugin
{
    private string assetBundleName = "OrbitPrefabBundle";
    private AssetBundle assetBundle;

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
            ReplaceCurrentPlanetPrefabs(scene.buildIndex);
        }
    }

    private void ReplaceCurrentPlanetPrefabs(int sceneLevelID)
    {
        if (assetBundle != null)
        {
            SelectableLevel[] selectableLevels = Resources.FindObjectsOfTypeAll<SelectableLevel>();

            foreach (SelectableLevel selectableLevel in selectableLevels)
            {
                int levelID = selectableLevel.levelID;
                int prefabNumber = GetPrefabNumberForLevel(levelID);

                string prefabName = $"Prefab{prefabNumber}";
                GameObject newPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

                if (newPrefab != null)
                {
                    selectableLevel.planetPrefab = newPrefab;
                    Debug.Log($"[NightSkyPlugin] Replaced currentPlanetPrefab for LevelID {levelID} with PrefabNumber {prefabNumber}");
                }
                else
                {
                    Debug.LogError($"[NightSkyPlugin] Prefab not found for LevelID {levelID} and PrefabNumber {prefabNumber} in AssetBundle");
                }
            }

            assetBundle.Unload(false);
        }
        else
        {
            Debug.LogError($"[NightSkyPlugin] AssetBundle is not loaded. Unable to replace prefabs.");
        }
    }

    private int GetPrefabNumberForLevel(int levelID)
    {
        switch (levelID)
        {
            case 0: return 1;
            case 1: return 1;
            case 2: return 2;
            case 3: return 4;
            case 4: return 2;
            case 5: return 3;
            case 6: return 3;
            case 7: return 1;
            case 8: return 3;
            default:
                Debug.LogError($"[NightSkyPlugin] Unknown level ID: {levelID}");
                return 0;
        }
    }
}
