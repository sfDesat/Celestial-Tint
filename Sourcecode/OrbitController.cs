using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class OrbitController : MonoBehaviour
{
    [Header("In Space Settings")]
    public Vector3 sunRotateVector = new Vector3(0.002f, 0, 0);
    public Vector3 planetRotateVector = new Vector3(0, 0, -0.0015f);
    public Vector3 planetRotateStartVector = new Vector3(0, 0, -10);
    [Space]
    public Vector3 planetPosition;
    public float orbitHeight;
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
    public Material planetMaterial;
    public CustomRenderTexture planetRenderTexture;

    [Header("References")]
    public GameObject sun;
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


    private void Start()
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
                planetPosition = new Vector3(0, -(sky.planetaryRadius.value + orbitHeight), 0);
                sky.planetCenterPosition.value = planetPosition;
                Debug.Log("[OrbitController] Set planet center to " + sky.planetCenterPosition.value);

                // Set MeteorShower
                //if (Random.Range(1, meteorShowerChance) == 1) meteorShower.SetActive(true);
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

        sky.planetRotation.value += planetRotateVector;
        //sky.spaceRotation.value += planetRotateVector;

        sky.planetCenterPosition.value = planetPosition;
    }
}
