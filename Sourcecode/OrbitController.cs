using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class OrbitController : MonoBehaviour
{
    [Header("In Space Settings")]
    public bool movePlanetOnly;
    public Vector3 sunRotateVector = new Vector3(0.002f, 0, 0);
    public Vector3 moonRotateVector = new Vector3(0.002f, 0, 0);
    public Vector3 planetRotateVector = new Vector3(0, 0, -0.002f);
    public Vector3 planetRotateStartVector = new Vector3(0, 0, -10);
    [Space]
    public Vector3 planetPosition;
    public float orbitHeight = 80000f;
    public float planetRadius = 6378100f;
    public Cubemap planetTexture;
    [Space]
    public float sunStartMin;
    public float sunStartMax;

    [Header("Colorization Settings")]
    public Color planetTint;
    public Color airTint;

    [Header("Material Settings")]
    public Material planetMaterial;
    public Cubemap assignedCubemap;

    [Header("References")]
    public GameObject sun;
    public GameObject moon;
    [Space]
    public Volume skyVolume;

    private PhysicallyBasedSky sky;

    private void Awake()
    {
        Debug.Log("[OrbitController] Awake started");

        // Get the Physically Based Sky
        if (skyVolume != null && skyVolume.profile != null)
        {
            if (skyVolume.profile.TryGet(out sky))
            {
                Debug.Log("[OrbitController] Physically Based Sky successfully obtained");

                // Apply starting rotation
                sun.transform.Rotate(Random.Range(sunStartMin, sunStartMax), 0, 0, Space.Self);
                sky.planetRotation.value = planetRotateStartVector;

                // Apply tints
                sky.groundTint.value = planetTint;
                sky.airTint.value = airTint;

                // Use the assigned Cubemap directly
                sky.groundColorTexture.value = assignedCubemap;
                sky.groundEmissionTexture.value = assignedCubemap;

                // Apply Orbit
                sky.planetCenterPosition.value = new Vector3(0, -(planetRadius + orbitHeight), 0);
                planetPosition = sky.planetCenterPosition.value;
            }
            else
            {
                Debug.LogError("[OrbitController] Physically Based Sky not found in profile");
            }
        }
        else
        {
            Debug.LogError("[OrbitController] SkyVolume or its profile is null");
        }

        Debug.Log("[OrbitController] Awake completed");
    }
}
