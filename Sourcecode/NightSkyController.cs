using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class NightSkyController : MonoBehaviour
{
    [Header("On Planet Settings")]
    public float starBrightness = 6f;

    [Header("References")]
    public Volume stormySkyVolume;
    public Volume eclipsedSkyVolume;
    public Volume clearSkyVolume;

    private PhysicallyBasedSky sky;

    public void Awake()
    {
        SetSettings();
    }

    private void OnValidate()
    {
        SetSettings();
    }

    private void FixedUpdate() // Change to AnimationEvent or something
    {
        if (sky != null)
        {
            sky.spaceEmissionMultiplier.value = starBrightness;
        }
        else
        {
            Debug.LogError("[NightSkyController] Sky is null. Unable to set spaceEmissionMultiplier.");
        }
    }

    public void SetSettings()
    {
        // Get the Physically Based Sky
        if (clearSkyVolume != null)
            clearSkyVolume.profile.TryGet(out sky);
        else if (stormySkyVolume != null)
            stormySkyVolume.profile.TryGet(out sky);
        else if (eclipsedSkyVolume != null)
            eclipsedSkyVolume.profile.TryGet(out sky);
        else
            Debug.LogError("[Night Sky] No Volumes where found");
    }
}
