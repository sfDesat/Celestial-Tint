using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class VanillaMode
{
    private static GameObject sunCopy;

    private static string sunName = "Sun";

    public static void Initialize()
    {
        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[CT VanillaMode] Parts Loading");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isInOrbit = CelestialTintStart.CheckSceneState();

        if (isInOrbit) ActivateSun();
        else DeactivateSun();
    }

    private static async void OnSceneUnloaded(Scene scene)
    {
        bool isInOrbit = await CelestialTintStart.DelayedCheckSceneStateAsync();

        if (isInOrbit) ActivateSun();
        else DeactivateSun();
    }

    private static void ActivateSun()
    {
        if (sunCopy != null)
        {
            sunCopy.SetActive(true);
            EnableLightComponent(sunCopy, true);
        }
        else
        {
            GameObject originalProp = GameObject.Find(sunName);
            if (originalProp != null)
            {
                sunCopy = Object.Instantiate(originalProp, originalProp.transform.position, originalProp.transform.rotation);
                sunCopy.name = sunName + "_copy";
                sunCopy.SetActive(true);
                EnableLightComponent(sunCopy, true);
            }
            else
            {
                Debug.LogError("[CT VanillaMode] GameObject with name '" + sunName + "' not found.");
            }
        }
    }

    private static void DeactivateSun()
    {
        if (sunCopy != null)
        {
            EnableLightComponent(sunCopy, false);
            sunCopy.SetActive(false);
        }
    }

    private static void EnableLightComponent(GameObject obj, bool enable)
    {
        Light lightComponent = obj.GetComponent<Light>();
        if (lightComponent != null)
        {
            lightComponent.enabled = enable;
        }
        else
        {
            Debug.LogError("[CT VanillaMode] No Light component found on GameObject '" + obj.name + "'.");
        }
    }
}
