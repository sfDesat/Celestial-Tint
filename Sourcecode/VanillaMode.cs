using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class VanillaMode
{
    private static string assetBundleName = "SunPrefabBundle";
    private static AssetBundle assetBundle;
    private static GameObject sunPrefab;

    public static void Initialize()
    {
        string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string bundlePath = Path.Combine(assemblyDirectory, assetBundleName);

        assetBundle = AssetBundle.LoadFromFile(bundlePath);

        sunPrefab = assetBundle.LoadAsset<GameObject>("CTSun");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleSceneRelay" && SceneManager.sceneCount == 1)
        {
            SpawnSun();
        }
        else
        {
            DestroySun();
        }
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SampleSceneRelay")
        {
            DestroySun();
        }
    }

    private static void SpawnSun()
    {
        if (sunPrefab != null)
        {
            GameObject sunObject = Object.Instantiate(sunPrefab);
            sunObject.name = "CTSun";
        }
        else
        {
            Debug.LogError("[CT VanillaMode] Sun prefab not found in the asset bundle.");
        }
    }

    private static void DestroySun()
    {
        GameObject sunObject = GameObject.Find("CTSun");
        if (sunObject != null)
        {
            Object.Destroy(sunObject);
        }

    }
}
