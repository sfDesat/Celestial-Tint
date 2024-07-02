using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public static class ShipPartsLoader
{
    private static List<GameObject> spacePropsCopies = new List<GameObject>();

    private static List<string> propNames = new List<string> { "ShipLightsPost", "OutsideShipRoom", "ThrusterBackLeft", "ThrusterBackRight", "ThrusterFrontLeft", "ThrusterFrontRight", "SideMachineryLeft", "SideMachineryRight", "ShipSupportBeams", "ShipSupportBeams.001", "MeterBoxDevice.001" };

    public static void Initialize()
    {
        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[CT ShipPartsLoader] Parts Loading");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isInOrbit = CelestialTintStart.CheckSceneState();

        if (isInOrbit) ActivateSpaceProps();
        else if (SceneManager.GetActiveScene().name == "SampleSceneRelay") DeactivateSpaceProps();
        else spacePropsCopies.Clear();
    }

    private static async void OnSceneUnloaded(Scene scene)
    {
        bool isInOrbit = await CelestialTintStart.DelayedCheckSceneStateAsync();

        if (isInOrbit) ActivateSpaceProps();
        else if (SceneManager.GetActiveScene().name == "SampleSceneRelay") DeactivateSpaceProps();
        else spacePropsCopies.Clear();
    }

    private static void ActivateSpaceProps()
    {
        foreach (string propName in propNames)
        {
            GameObject copy = spacePropsCopies.Find(c => c.name == propName + "_copy");
            if (copy != null)
            {
                copy.SetActive(true);
            }
            else
            {
                GameObject originalProp = GameObject.Find(propName);
                if (originalProp != null)
                {
                    copy = Object.Instantiate(originalProp, originalProp.transform.position, originalProp.transform.rotation);
                    copy.name = propName + "_copy";
                    copy.SetActive(true);
                    spacePropsCopies.Add(copy);
                }
                else
                {
                    Debug.LogError("[CT ShipPartsLoader] GameObject with name '" + propName + "' not found.");
                }
            }
        }
    }

    private static void DeactivateSpaceProps()
    {
        foreach (GameObject copy in spacePropsCopies)
        {
            copy.SetActive(false);
        }
    }
}
