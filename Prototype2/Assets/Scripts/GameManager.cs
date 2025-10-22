using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameOverUi;
    public GameObject countDown;
    public float countdownSeconds = 3f;
    private TextMeshProUGUI countdownText;

    public bool IsGameOver { get; private set; }
    public event Action OnGameOver;
    void Awake()
    {
        Time.timeScale = 0f;
    }

    void Start()
    {
        countdownText = countDown.GetComponentInChildren<TextMeshProUGUI>();

        StartCoroutine(DoCountdown());
    }

    IEnumerator DoCountdown()
    {
        if (countDown != null)
        {
            countDown.SetActive(true);

            float t = countdownSeconds;
            while (t > 0f)
            {
                countdownText.text = Mathf.CeilToInt(t).ToString();
                yield return new WaitForSecondsRealtime(1f);
                t -= 1f;
            }

            countdownText.text = "GO!";
            yield return new WaitForSecondsRealtime(0.6f);

            countDown.SetActive(false);
        }

        // Resume the game
        Time.timeScale = 1f;
    }

    public void ShowGameOverScreen()
    {
        if (IsGameOver) return;
            IsGameOver = true;

        if (gameOverUi != null)
        {
            gameOverUi.SetActive(true);
        }

        OnGameOver?.Invoke();         // <¡X notify listeners
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}


