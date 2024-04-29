using HarmonyLib;
using UnityEngine;

public static class ShipDoorLoader
{
    public static void Initialize()
    {
        if (CelestialTint.ModConfig.ShipDoorAccess.Value)
        {
            var harmony = new Harmony("com.celestialtint.mod");
            harmony.PatchAll(typeof(CTDoorHangarShipDoor));
            harmony.PatchAll(typeof(CTDoorStartOfRound));

            if (CelestialTint.ModConfig.DebugLogging.Value) Debug.Log("[CT Ship Door Loader] Ship doors opened");
        }
    }


    [HarmonyPatch(typeof(HangarShipDoor), "Update")]
    private class CTDoorHangarShipDoor
    {
        private static void Prefix(HangarShipDoor __instance)
        {
            if (!__instance.buttonsEnabled)
            {
                __instance.SetDoorButtonsEnabled(true);
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "TeleportPlayerInShipIfOutOfRoomBounds")]
    private class CTDoorStartOfRound
    {
        private static bool Prefix(StartOfRound __instance)
        {
            return !__instance.inShipPhase;
        }
    }
}
