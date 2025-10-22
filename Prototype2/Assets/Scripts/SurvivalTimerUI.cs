using UnityEngine;
using TMPro;

public class SurvivalTimerUI : MonoBehaviour
{
    [Header("Refs")]
    public GameManager gameManager;         // drag in Inspector
    public TextMeshProUGUI scoreText;

    public float Elapsed { get; private set; }
    bool running;

    void Start()
    {
        // Start after countdown (when timeScale becomes 1)
        StartCoroutine(StartWhenUnpaused());

        // Stop when GameManager says game over
        if (gameManager != null)
            gameManager.OnGameOver += StopTimer;
    }

    System.Collections.IEnumerator StartWhenUnpaused()
    {
        yield return new WaitUntil(() => Time.timeScale > 0f);
        running = true;
    }

    void OnDestroy()
    {
        if (gameManager != null)
            gameManager.OnGameOver -= StopTimer;
    }

    void Update()
    {
        if (!running) return;
        Elapsed += Time.deltaTime;
        if (scoreText) scoreText.text = $"Time: {Elapsed:0.0}s";
    }

    public void StopTimer() => running = false;   // called by GameManager event
    public void ResetTimer() { Elapsed = 0f; running = false; }
}
