using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.3.0")]
public class CelestialTint : BaseUnityPlugin
{
    internal static CTConfig ModConfig; // Change to internal accessibility

    private void Awake()
    {
        Debug.Log("[Celestial Tint] Loading complete");

        // Load config
        ModConfig = new CTConfig(Config); // Pass ConfigFile instance to CTConfig

        // Initialize the loader
        CelestialTintLoader.Initialize();
    }
}
