using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("UI")]
    public Image healthBarFill; 

    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            //Calculating the percentage from 0 to 1 (for example, 5 out of 10 lives = 0.5)
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }


    void Die()
    {
        isDead = true;
        Debug.Log("Guard defeated!");

        //Notify the MenuManager (only within the SamuraiScene)
        if (SceneManager.GetActiveScene().name == "SamuraiScene")
        {
            MenuManager menuManager = Object.FindAnyObjectByType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.EnemyDefeat();
            }
        }
        else
        {
            Debug.Log("Guard in the NinjaScene defeated! - no scene change");
        }

        //1. Disable NavMeshAgent so it stops moving
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        //2. Stopping the BehaviorGraph, so that the AI stopps to act
        BehaviorGraphAgent behaviorAgent = GetComponent<BehaviorGraphAgent>();
        if (behaviorAgent != null) behaviorAgent.enabled = false;

        //3. "Falling over" - tipping the guard to the side (adjust rotation) 
        transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        //Moving it down a bit so that it lies flat on the ground
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z);
    }

    public bool IsDead()
    {
        return isDead;
    }
}