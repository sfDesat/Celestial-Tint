using BepInEx;
using UnityEngine;

[BepInPlugin("NightSkyPlugin", "Night Sky Plugin", "1.0.7")]
public class NightSkyPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Debug.Log("[NightSkyPlugin] Nightsky loaded");

        // Initialize the NightSkyManager
        NightSkyManager.Initialize();
    }
}
