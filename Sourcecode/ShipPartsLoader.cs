using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class ShipPartsLoader
{
    private static bool isSampleSceneRelayLoaded = false;
    private static List<GameObject> spacePropsCopies = new List<GameObject>();

    // Define the propNames list here
    private static List<string> propNames = new List<string> { "ShipLightsPost", "OutsideShipRoom", "ThrusterBackLeft", "ThrusterBackRight", "ThrusterFrontLeft", "ThrusterFrontRight", "SideMachineryLeft", "SideMachineryRight", "ShipSupportBeams", "ShipSupportBeams.001", "MeterBoxDevice.001" };

    public static void Initialize()
    {
        if (CelestialTint.ModConfig.DisplayShipParts.Value)
        {
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Parts Loading");

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            if (isSampleSceneRelayLoaded)
            {
                ActivateSpaceProps();
            }
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleSceneRelay" && SceneManager.sceneCount == 1)
        {
            isSampleSceneRelayLoaded = true;
            ActivateSpaceProps();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Activating Parts");
        }
        else
        {
            isSampleSceneRelayLoaded = false;
            DeactivateSpaceProps();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Deactivating Parts");
        }
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SampleSceneRelay")
        {
            isSampleSceneRelayLoaded = false;
            DeactivateSpaceProps();
        }
    }

    private static void ActivateSpaceProps()
    {
        foreach (string propName in propNames)
        {
            GameObject originalProp = GameObject.Find(propName);
            if (originalProp != null)
            {
                GameObject copy = Object.Instantiate(originalProp, originalProp.transform.position, originalProp.transform.rotation);
                spacePropsCopies.Add(copy);
                copy.SetActive(true);
            }
            else
            {
                Debug.LogError("GameObject with name '" + propName + "' not found.");
            }
        }
    }

    private static void DeactivateSpaceProps()
    {
        foreach (GameObject copy in spacePropsCopies)
        {
            Object.Destroy(copy);
        }
        spacePropsCopies.Clear();
    }
}
