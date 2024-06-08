using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class VanillaMode
{
    private static GameObject sunCopy;

    private static string sunName = "Sun";

    public static void Initialize()
    {
        if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT VanillaMode] Parts Loading");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Check the initial state of the scene
        VanillaMode.CheckSceneState();
    }

    private static void CheckSceneState()
    {
        // Check if SampleSceneRelay is the only scene loaded
        if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == "SampleSceneRelay")
        {
            ActivateSun();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT VanillaMode] Activating Sun");
        }
        else if (SceneManager.GetActiveScene().name == "SampleSceneRelay")
        {
            DeactivateSun();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT VanillaMode] Deactivating Sun");
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check the scene state after it's loaded
        CheckSceneState();
    }

    private static IEnumerator DelayedCheckSceneState()
    {
        // Wait for the current frame to end before checking the scene state
        yield return new WaitForEndOfFrame();
        CheckSceneState();
    }

    private static void OnSceneUnloaded(Scene scene)
    {
        // Delay the execution of CheckSceneState to ensure the scene has been fully unloaded
        CoroutineHandlerSun.Instance.StartCoroutine(DelayedCheckSceneState());
    }

    private static void ActivateSun()
    {
        if (sunCopy != null)
        {
            sunCopy.SetActive(true); // Reactivate the existing copy
            EnableLightComponent(sunCopy, true); // Ensure the Light component is active
        }
        else
        {
            GameObject originalProp = GameObject.Find(sunName);
            if (originalProp != null)
            {
                sunCopy = Object.Instantiate(originalProp, originalProp.transform.position, originalProp.transform.rotation);
                sunCopy.name = sunName + "_copy"; // Set a distinct name for the copy
                sunCopy.SetActive(true);
                EnableLightComponent(sunCopy, true); // Ensure the Light component is active
            }
            else
            {
                Debug.LogError("GameObject with name '" + sunName + "' not found.");
            }
        }
    }

    private static void DeactivateSun()
    {
        if (sunCopy != null)
        {
            EnableLightComponent(sunCopy, false); // Ensure the Light component is inactive
            sunCopy.SetActive(false); // Deactivate the copy
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
            Debug.LogError("No Light component found on GameObject '" + obj.name + "'.");
        }
    }

    // Coroutine handler to allow starting coroutines in static methods
    public class CoroutineHandlerSun : MonoBehaviour
    {
        private static CoroutineHandlerSun _instance;
        public static CoroutineHandlerSun Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CoroutineHandlerSun").AddComponent<CoroutineHandlerSun>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }
}
