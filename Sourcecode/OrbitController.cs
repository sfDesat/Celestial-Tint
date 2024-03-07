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

    [Header("References")]
    public GameObject sun;
    public GameObject moon;
    [Space]
    public Volume skyVolume;

    private PhysicallyBasedSky sky;

    public void Awake()
    {
        Debug.Log("[OrbitController] Awake started");

        // Get the Physically Based Sky
        if (skyVolume != null)
        {
            if (skyVolume.profile != null)
            {
                skyVolume.profile.TryGet(out sky);

                if (sky != null)
                {
                    Debug.Log("[OrbitController] Physically Based Sky successfully obtained");

                    // Apply starting rotation
                    sun.transform.Rotate(Random.Range(sunStartMin, sunStartMax), 0, 0, Space.Self);
                    sky.planetRotation.value = planetRotateStartVector;

                    // Apply tints
                    sky.groundTint.value = planetTint;
                    sky.airTint.value = airTint;

                    // Apply textures
                    sky.groundColorTexture.value = planetTexture;
                    sky.groundEmissionTexture.value = planetTexture;

                    // Apply Orbit
                    sky.planetCenterPosition.value = new(0, -(planetRadius + orbitHeight), 0);
                    planetPosition = sky.planetCenterPosition.value;
                }
                else
                {
                    Debug.LogError("[OrbitController] Physically Based Sky not found in profile");
                }
            }
            else
            {
                Debug.LogError("[OrbitController] SkyVolume profile is null");
            }
        }
        else
        {
            Debug.LogError("[OrbitController] SkyVolume is null");
        }

        Debug.Log("[OrbitController] Awake completed");
    }

    private void FixedUpdate()
    {
        SpaceSimulation();
    }

    private void SpaceSimulation()
    {
        sky.planetRotation.value += planetRotateVector;
        sky.planetCenterPosition.value = planetPosition;

        if (!movePlanetOnly)
        {
            if (moon != null)
                moon.transform.Rotate(moonRotateVector);

            sun.transform.Rotate(sunRotateVector);
            sky.spaceRotation.value += planetRotateVector;
        }
    }
}
