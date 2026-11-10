using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Wichtig für das neue Input System

public class MenuManager : MonoBehaviour
{

    [Header("Menü Panels")]
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private int remainingEnemies;

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

        //Count the Enemies at the start
        remainingEnemies =GameObject.FindGameObjectsWithTag("Enemy").Length;
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

    public void EnemyDefeat()
    {
        remainingEnemies--; //reduces the Enemy count every time an enemy dies.

        //when there are no Enemies left the next scene is suppost to be called
        if(remainingEnemies <= 0)
        {
            Invoke("LoadNinjaScene", 1.5f);
        }
    }

    void LoadNinjaScene()
    {
        Time.timeScale = 1f; //reset the time to the normal scale
        SceneManager.LoadScene("NinjaScene");
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

    public void LoadVictoryScene()
    {
        Time.timeScale = 1f; // Zeit zurücksetzen
        SceneManager.LoadScene(4); // Lädt die Victory-Szene
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Spiel wird beendet...");
    }
}