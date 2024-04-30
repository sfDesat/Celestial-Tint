using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public static class ShipPartsLoader
{
    private static List<GameObject> spacePropsCopies = new List<GameObject>();

    // Define the propNames list here
    private static List<string> propNames = new List<string> { "ShipLightsPost", "OutsideShipRoom", "ThrusterBackLeft", "ThrusterBackRight", "ThrusterFrontLeft", "ThrusterFrontRight", "SideMachineryLeft", "SideMachineryRight", "ShipSupportBeams", "ShipSupportBeams.001", "MeterBoxDevice.001" };

    public static void Initialize()
    {
        if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Parts Loading");

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Check the initial state of the scene
        CheckSceneState();
    }

    private static void CheckSceneState()
    {
        // Check if SampleSceneRelay is the only scene loaded
        if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == "SampleSceneRelay")
        {
            ActivateSpaceProps();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Activating Parts");
        }
        else if (SceneManager.GetActiveScene().name == "SampleSceneRelay")
        {
            DeactivateSpaceProps();
            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Parts Loader] Deactivating Parts");
        }
        else
        {
            spacePropsCopies.Clear();
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
        CoroutineHandler.Instance.StartCoroutine(DelayedCheckSceneState());
    }

    private static void ActivateSpaceProps()
    {
        foreach (string propName in propNames)
        {
            GameObject copy = spacePropsCopies.Find(c => c.name == propName + "_copy");
            if (copy != null)
            {
                copy.SetActive(true); // Reactivate the existing copy
            }
            else
            {
                GameObject originalProp = GameObject.Find(propName);
                if (originalProp != null)
                {
                    copy = Object.Instantiate(originalProp, originalProp.transform.position, originalProp.transform.rotation);
                    copy.name = propName + "_copy"; // Set a distinct name for the copy
                    copy.SetActive(true);
                    spacePropsCopies.Add(copy); // Add the copy to the list
                }
                else
                {
                    Debug.LogError("GameObject with name '" + propName + "' not found.");
                }
            }
        }
    }

    private static void DeactivateSpaceProps()
    {
        foreach (GameObject copy in spacePropsCopies)
        {
            copy.SetActive(false); // Deactivate the copy
        }
    }


    // Coroutine handler to allow starting coroutines in static methods
    public class CoroutineHandler : MonoBehaviour
    {
        private static CoroutineHandler _instance;
        public static CoroutineHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CoroutineHandler").AddComponent<CoroutineHandler>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }
}
