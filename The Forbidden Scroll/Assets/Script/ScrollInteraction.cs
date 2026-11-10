using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrollInteraction : MonoBehaviour
{
    public float interactionRange = 3f; //how close the player has to stand to the scroll for it to work
    private Transform playerTransform; //for calculation the distance

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
                    // Fallback, falls kein MenuManager da ist
                    SceneManager.LoadScene(4);
                }
                // deletes the scroll from the game world
                Destroy(gameObject);

            }
        }
    }
}