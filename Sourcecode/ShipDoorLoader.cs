using HarmonyLib;
using UnityEngine;

public static class ShipDoorLoader
{
    public static void Initialize()
    {
        var harmony = new Harmony("com.celestialtint.mod");
        harmony.PatchAll(typeof(CTDoorHangarShipDoor));
        harmony.PatchAll(typeof(CTDoorStartOfRound));

        if (CelestialTintStart.ModConfig.DebugLogging.Value) Debug.Log("[CT ShipDoorLoader] Ship doors opened");
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
