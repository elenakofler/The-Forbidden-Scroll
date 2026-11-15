using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class MenuManager : MonoBehaviour
{
    [Header("Menü Panels")]
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject hudUI;

    private bool isPaused = false;
    private int remainingEnemies;

    void Start()
    {
        //Enable the start menu when launching the game and pause the time
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
        remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    void Update()
    {
        //Querying the new Input System
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
        if (hudUI != null) hudUI.SetActive(true); //showing HUD again if game is unpaused
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (hudUI != null) hudUI.SetActive(false); //Hiding HUD if game is paused
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

    public void SamuraiScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(2);
    }

    public void LoadVictoryScene()
    {
        Time.timeScale = 1f; //Reset time
        SceneManager.LoadScene(4); //Load Victory-Scene
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is ending...");
    }
}