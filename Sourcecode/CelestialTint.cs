using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("CelestialTint", "Celestial Tint", "1.4.2")]
public class CelestialTint : BaseUnityPlugin
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

        // Load config
        ModConfig = new CTConfig(Config);

        if (CelestialTint.ModConfig.VanillaMode.Value) VanillaMode.Initialize();
        if (!CelestialTint.ModConfig.VanillaMode.Value) CelestialTintLoader.Initialize();
        if (CelestialTint.ModConfig.DisplayShipParts.Value) ShipPartsLoader.Initialize();
        if (CelestialTint.ModConfig.ShipDoorAccess.Value) ShipDoorLoader.Initialize();
    }
}
