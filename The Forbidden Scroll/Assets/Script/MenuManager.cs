using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Wichtig für das neue Input System

public class MenuManager : MonoBehaviour
{
    [Header("Menü Panels")]
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        // Start-Menü beim Spielstart einschalten und Zeit anhalten
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(true);
            Time.timeScale = 0f; 
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Korrekte Abfrage für das neue Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (startMenuPanel == null || !startMenuPanel.activeSelf)
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Spiel wird beendet...");
    }
}