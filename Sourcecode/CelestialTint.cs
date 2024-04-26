using BepInEx;
using UnityEngine;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.1.13")]
public class CelestialTint : BaseUnityPlugin
{
    private void Awake()
    {
        Debug.Log("[Celestial Tint] Loading complete");

        // Initialize the loader
        CelestialTintLoader.Initialize();
    }
}
