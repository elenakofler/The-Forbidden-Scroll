using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrollInteraction : MonoBehaviour
{
    public float interactionRange = 3f; //how close the player has to stand to the scroll for it to work
    private Transform playerTransform; //for calculation the distance
    private bool playerIsClose = false; //to see if the player is in range (fixed typo here)

    //looks for the tag player and safes the transformation component
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }


    void Update()
    {
        if (playerTransform == null) return;

        //messures distance between player abd scroll
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionRange)
        {
            playerIsClose = true; //the player is now close enough

            // we should put a UI hint like : Press F to pick up scroll
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                // Calls the method in NinjaController to get the scroll
                playerTransform.GetComponent<NinjaController>().CollectScroll();
                
                MenuManager menuManager = Object.FindAnyObjectByType<MenuManager>();
                if (menuManager != null)
                {
                    menuManager.LoadVictoryScene();
                }
                else
                {
                    //loads the VictoryScene
                    SceneManager.LoadScene(4);
                }
                // deletes the scroll from the game world
                Destroy(gameObject);
            }
        }
        else
        {
            playerIsClose = false;
        }
    } 

    
    void OnGUI()
    {
        if (playerIsClose)
        {
            //size and position of the text
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            
            Rect rect = new Rect(screenWidth / 2 - 150, screenHeight - 150, 300, 50);

            //style of the text, etc
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 25;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.red; 

            // the actual text 
            GUI.Label(rect, "Press [F] to pick up", style);
        }
    }
}