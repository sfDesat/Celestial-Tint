using System;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.5.2")]
public class CelestialTintStart : BaseUnityPlugin
{
    internal static CTConfig ModConfig;

    private void Awake()
    {
        var sssAssembly = AppDomain.CurrentDomain.GetAssemblies();
        bool isSpaceSunShineLoaded = sssAssembly.Any(assembly => assembly.FullName.StartsWith("SpaceSunShine"));
        if (isSpaceSunShineLoaded) Debug.LogError("[Celestial Tint] Incompatible mod found: SpaceSunShine!");

        var ssdAssembly = AppDomain.CurrentDomain.GetAssemblies();
        bool isSpaceShipDoorLoaded = ssdAssembly.Any(assembly => assembly.FullName.StartsWith("SpaceShipDoor"));
        if (isSpaceShipDoorLoaded) Debug.LogWarning("[Celestial Tint] Semi-incompatible mod found: SpaceShipDoor!");

        Debug.Log("[Celestial Tint] Loading complete");

        ModConfig = new CTConfig(Config);

        if (CelestialTintStart.ModConfig.VanillaMode.Value) VanillaMode.Initialize();
        if (!CelestialTintStart.ModConfig.VanillaMode.Value) CelestialTintLoader.Initialize();
        if (CelestialTintStart.ModConfig.DisplayShipParts.Value) ShipPartsLoader.Initialize();
        if (CelestialTintStart.ModConfig.ShipDoorAccess.Value) ShipDoorLoader.Initialize();
    }

    public static bool CheckSceneState()
    {
        bool isInOrbit = SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().name == "SampleSceneRelay";
        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[Celestial Tint] " + (isInOrbit ? "In Orbit" : "Out Orbit"));
        
        return isInOrbit;
    }

    public static async Task<bool> DelayedCheckSceneStateAsync()
    {
        await Task.Yield(); // Wait for the next frame
        return CheckSceneState();
    }

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
