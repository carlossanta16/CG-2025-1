using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [Header("Rotation Speed")]
    [Tooltip("Rotation speed in degrees per second")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // Modify this in Inspector

    void Update()
    {
        // Rotate based on speed and deltaTime for smooth rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
