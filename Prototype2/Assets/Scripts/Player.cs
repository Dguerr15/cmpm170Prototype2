using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 5f;
    private bool isGameOver = false;
    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        transform.Translate(movement.normalized * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleGameOver();
            
        }
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }
}
