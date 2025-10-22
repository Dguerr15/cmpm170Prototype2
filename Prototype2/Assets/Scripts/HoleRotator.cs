using UnityEngine;

public class HoleRotator : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // default: spins around Y

    [Header("Rotation Mode")]
    public bool localRotation = true; // true = use local space, false = world space

    void Update()
    {
        if (localRotation)
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        else
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
    }
}
