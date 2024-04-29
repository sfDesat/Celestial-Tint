using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.3.1")]
public class CelestialTint : BaseUnityPlugin
{
    internal static CTConfig ModConfig;

    private void Awake()
    {
        Debug.Log("[Celestial Tint] Loading complete");

        // Load config
        ModConfig = new CTConfig(Config);

        // Initialize the loader
        CelestialTintLoader.Initialize();
        ShipPartsLoader.Initialize();
        ShipDoorLoader.Initialize();
    }
}
