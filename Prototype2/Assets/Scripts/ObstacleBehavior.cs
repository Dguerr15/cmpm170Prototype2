using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    public float fallSpeed = 5f;
    private float destoryYPosition = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * fallSpeed * Time.deltaTime);

        if (transform.position.y > destoryYPosition)
        {
            Destroy(gameObject);
        }
    }

    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
    }
}
