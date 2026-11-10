using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class GuardHealth : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Wache hat Schaden erhalten! Restleben: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Wache wurde besiegt!");

        // --- HIER WIRD DER MENUMANAGER INFORMIERT ---
        MenuManager menuManager = Object.FindAnyObjectByType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.EnemyDefeat();
        }

        // 1. NavMeshAgent ausschalten, damit sie aufhört sich zu bewegen
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // 2. Behavior Graph stoppen, damit die KI aufhört zu agieren
        BehaviorGraphAgent behaviorAgent = GetComponent<BehaviorGraphAgent>();
        if (behaviorAgent != null) behaviorAgent.enabled = false;

        // 3. "Umfallen": Die Wache zur Seite kippen (Rotation anpassen)
        transform.rotation = Quaternion.Euler(90f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        // Etwas nach unten versetzen, damit sie flach auf dem Boden liegt (abhängig vom Modell)
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.4f, transform.position.z);

        // 4. Collider ausschalten (optional), damit man durch den Körper laufen kann
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public bool IsDead()
    {
        return isDead;
    }
}