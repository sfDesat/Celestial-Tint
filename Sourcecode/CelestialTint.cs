using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.4.1")]
public class CelestialTint : BaseUnityPlugin
{
    internal static CTConfig ModConfig;

    private void Awake()
    {
        Debug.Log("[Celestial Tint] Loading complete");

        // Load config
        ModConfig = new CTConfig(Config);

        if (CelestialTint.ModConfig.VanillaMode.Value) VanillaMode.Initialize();
        if (!CelestialTint.ModConfig.VanillaMode.Value) CelestialTintLoader.Initialize();
        if (CelestialTint.ModConfig.DisplayShipParts.Value) ShipPartsLoader.Initialize();
        if (CelestialTint.ModConfig.ShipDoorAccess.Value) ShipDoorLoader.Initialize();
    }
}
