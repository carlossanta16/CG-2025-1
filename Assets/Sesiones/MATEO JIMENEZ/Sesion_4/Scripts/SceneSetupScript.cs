using UnityEngine;

public class SetupWaterScene : MonoBehaviour
{
    [Header("Scene References")]
    public GameObject waterPlane;
    public Camera mainCamera;
    
    [Header("Water Material")]
    public Material waterMaterial;
    public Cubemap environmentCubemap;
    public Texture2D normalMapTexture;
    public Texture2D foamTexture;
    public Texture2D noiseTexture;
    
    [Header("Scene Setup")]
    public GameObject[] objectsToPlaceInWater;
    public float waterLevel = 0f;
    
    void Start()
    {
        // Make sure we have necessary references
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        // Enable depth texture mode on camera
        if (mainCamera != null)
            mainCamera.depthTextureMode = DepthTextureMode.Depth;
        
        // Setup water material if provided
        if (waterMaterial != null && waterPlane != null)
        {
            Renderer waterRenderer = waterPlane.GetComponent<Renderer>();
            if (waterRenderer != null)
            {
                // Apply material to water plane
                waterRenderer.material = waterMaterial;
                
                // Apply textures if they exist
                if (environmentCubemap != null)
                    waterMaterial.SetTexture("_Cubemap", environmentCubemap);
                
                if (normalMapTexture != null)
                    waterMaterial.SetTexture("_NormalMap", normalMapTexture);
                    
                if (foamTexture != null)
                    waterMaterial.SetTexture("_FoamTexture", foamTexture);
                    
                if (noiseTexture != null)
                    waterMaterial.SetTexture("_NoiseTexture", noiseTexture);
            }
        }
        
        // Place objects in water
        PlaceObjectsInWater();
    }
    
    void PlaceObjectsInWater()
    {
        if (objectsToPlaceInWater == null || objectsToPlaceInWater.Length == 0)
            return;
            
        foreach (GameObject obj in objectsToPlaceInWater)
        {
            if (obj != null)
            {
                // Position objects so they're partially submerged in water
                Vector3 position = obj.transform.position;
                Renderer renderer = obj.GetComponent<Renderer>();
                
                if (renderer != null)
                {
                    // Calculate bounds to place object with bottom at water level
                    Bounds bounds = renderer.bounds;
                    float bottomY = bounds.min.y;
                    float heightOffset = position.y - bottomY;
                    
                    // Move object so bottom is at water level plus small random offset
                    float randomOffset = Random.Range(-0.2f, 0.2f);
                    position.y = waterLevel + heightOffset + randomOffset;
                    obj.transform.position = position;
                    
                    // Add small random rotation
                    obj.transform.rotation = Quaternion.Euler(
                        Random.Range(-5f, 5f),
                        Random.Range(0f, 360f),
                        Random.Range(-5f, 5f)
                    );
                }
            }
        }
    }
}