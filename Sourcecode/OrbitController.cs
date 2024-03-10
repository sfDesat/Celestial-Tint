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
    [Space]
    public float sunStartMin;
    public float sunStartMax;
    [Space]
    public GameObject meteorShower;
    public float meteorShowerChance = 3f;

    [Header("Colorization Settings")]
    public Texture2D planetTexture; 
    public Color planetTint;
    public Color airTint;

    [Header("Material Settings")]
    public Material planetMaterial; // Use the material with the ShaderGraph
    public CustomRenderTexture planetRenderTexture;

    [Header("References")]
    public GameObject sun;
    public GameObject moon;
    [Space]
    public Volume skyVolume;

    private PhysicallyBasedSky sky;

    private void OnValidate()
    {
        Debug.Log("[OrbitController] OnVoid called");

        // Set color in the shader
        if (planetMaterial != null)
        {
            planetMaterial.SetColor("_SurfaceColor", planetTint);
            planetMaterial.SetTexture("_SurfaceTexture", planetTexture);

            Debug.Log("[OrbitController] Shader properties set successfully");
        }
        else
        {
            Debug.LogError("[OrbitController] planetMaterial is null. Cannot set shader properties.");
        }

        planetRenderTexture.Update();
        planetRenderTexture.Initialize();
    }


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
                sky.airTint.value = airTint;
                sky.groundTint.value = planetTint;

                planetRenderTexture.Update();
                planetRenderTexture.Initialize();

                // Set color in the shader
                planetMaterial.SetColor("_SurfaceColor", planetTint);
                planetMaterial.SetTexture("_SurfaceTexture", planetTexture);

                // Apply Orbit
                sky.planetCenterPosition.value = new Vector3(0, -(planetRadius + orbitHeight), 0);
                planetPosition = sky.planetCenterPosition.value;

                // Set MeteorShower
                if (Random.Range(1, meteorShowerChance) == 2)
                    meteorShower.active = true;
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

    public void FixedUpdate()
    {
        sun.transform.Rotate(sunRotateVector);
        moon.transform.Rotate(moonRotateVector);

        sky.planetRotation.value += planetRotateVector;

        if (planetRenderTexture != null)
        {
            //planetRenderTexture.Update();
            //planetRenderTexture.Initialize();
            //Debug.Log("CRT applied");
        }
    }
}
