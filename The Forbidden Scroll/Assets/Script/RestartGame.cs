using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Restart()
    {
        Debug.Log("RESTART BUTTON CLICKED!");
        SceneManager.LoadScene("SamuraiScene");
    }

}
