using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ImpactStopper : MonoBehaviour
{
    public string wallTag = "Wall";   // tag your hole/walls as "Wall"
    public float stopThreshold = 2f;  // only stop if impact is hard enough

    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void OnCollisionEnter(Collision c)
    {
        if (!c.collider.CompareTag(wallTag)) return;

        // Use impact speed (relative velocity) to decide if it was a "hard" hit
        if (c.relativeVelocity.magnitude >= stopThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
