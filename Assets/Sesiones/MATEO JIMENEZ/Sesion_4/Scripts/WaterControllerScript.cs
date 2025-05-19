using UnityEngine;

public class WaterController : MonoBehaviour
{
    // Public properties that will be exposed in the inspector
    [Header("References")]
    public Material waterMaterial;
    public Transform cameraTransform;
    
    [Header("Wave Settings")]
    [Range(0.0f, 1.0f)]
    public float waveAmplitude = 0.2f;
    [Range(0.0f, 10.0f)]
    public float waveFrequency = 2.0f;
    [Range(0.0f, 5.0f)]
    public float waveSpeed = 1.0f;
    
    [Header("Refraction Settings")]
    [Range(0.0f, 1.0f)]
    public float refractionStrength = 0.2f;
    
    [Header("Reflection Settings")]
    [Range(0.0f, 1.0f)]
    public float reflectionStrength = 0.5f;
    [Range(0.0f, 1.0f)]
    public float glossiness = 0.8f;
    [Range(1.0f, 10.0f)]
    public float fresnelPower = 5.0f;
    
    [Header("Depth Settings")]
    [Range(0.0f, 10.0f)]
    public float depthDistance = 2.0f;
    [Range(0.0f, 10.0f)]
    public float depthFalloff = 2.0f;
    
    [Header("Foam Settings")]
    public Color foamColor = Color.white;
    [Range(0.0f, 2.0f)]
    public float intersectionFoamDepth = 0.5f;
    [Range(0.0f, 1.0f)]
    public float foamCutoff = 0.8f;
    [Range(0.0f, 10.0f)]
    public float foamScale = 5.0f;
    [Range(0.0f, 1.0f)]
    public float waveFoamCutoff = 0.9f;
    
    // Start is called before the first frame update
    void Start()
    {
        // Make sure we have references to necessary components
        if (waterMaterial == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                waterMaterial = renderer.material;
            }
        }
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Enable depth texture on the camera
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        if (waterMaterial != null)
        {
            // Update shader parameters based on our public properties
            waterMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
            waterMaterial.SetFloat("_WaveFrequency", waveFrequency);
            waterMaterial.SetFloat("_WaveSpeed", waveSpeed);
            waterMaterial.SetFloat("_RefractionStrength", refractionStrength);
            waterMaterial.SetFloat("_ReflectionStrength", reflectionStrength);
            waterMaterial.SetFloat("_Glossiness", glossiness);
            waterMaterial.SetFloat("_FresnelPower", fresnelPower);
            waterMaterial.SetFloat("_DepthDistance", depthDistance);
            waterMaterial.SetFloat("_DepthFalloff", depthFalloff);
            waterMaterial.SetColor("_FoamColor", foamColor);
            waterMaterial.SetFloat("_IntersectionFoamDepth", intersectionFoamDepth);
            waterMaterial.SetFloat("_FoamCutoff", foamCutoff);
            waterMaterial.SetFloat("_FoamScale", foamScale);
            waterMaterial.SetFloat("_WaveFoamCutoff", waveFoamCutoff);
        }
    }
}