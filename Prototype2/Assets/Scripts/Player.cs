using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 5f;
    private bool isGameOver = false;
    private Rigidbody camRb;

    public GameManager gameManager; //referance to the gameManager object to restart game after death
  // Update is called once per frame
    void Start()
    {
        camRb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (isGameOver) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
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
        camRb.useGravity = true;
        camRb.constraints = RigidbodyConstraints.None;
        camRb.AddTorque(Random.insideUnitSphere * moveSpeed);
        //Time.timeScale = 0f;
        if (gameManager != null)
        {
            gameManager.ShowGameOverScreen();
        }
        Debug.Log("Game Over!");
    }
}
