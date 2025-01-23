using UnityEngine;

public class RandomRotationAndMovement : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

    [Header("Y Movement Settings")]
    [SerializeField] private float minY = 0f; // Minimum Y position
    [SerializeField] private float maxY = 5f; // Maximum Y position

    [Header("Floating Children Settings")]
    [SerializeField] private float floatRadius = 2f; // Radius of floating sphere
    [SerializeField] private float floatSpeed = 1f; // Speed of floating movement

    private float targetY;

    private void Start()
    {
        SetRandomYPosition();
    }

    private void Update()
    {
        HandleRotation();
        HandleYMovement();
        HandleChildrenFloatingIndependently();
    }

    private void HandleRotation()
    {
        // Rotate continuously on all axes
        transform.Rotate(new Vector3(1, 1, 1) * rotationSpeed * Time.deltaTime);
    }

    private void HandleYMovement()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = Mathf.Lerp(currentPosition.y, targetY, Time.deltaTime);
        transform.position = currentPosition;

        // If close to the target Y position, pick a new random position
        if (Mathf.Abs(currentPosition.y - targetY) < 0.1f)
        {
            SetRandomYPosition();
        }
    }

    private void HandleChildrenFloatingIndependently()
    {
        foreach (Transform child in transform)
        {
            Vector3 offset = new Vector3(
                Mathf.Sin(Time.time * floatSpeed + child.GetInstanceID() * 0.1f) * floatRadius,
                Mathf.Cos(Time.time * floatSpeed + child.GetInstanceID() * 0.2f) * floatRadius,
                Mathf.Sin(Time.time * floatSpeed * 0.5f + child.GetInstanceID() * 0.3f) * floatRadius
            );

            child.localPosition = offset;
        }
    }

    private void SetRandomYPosition()
    {
        targetY = Random.Range(minY, maxY);
    }
}
