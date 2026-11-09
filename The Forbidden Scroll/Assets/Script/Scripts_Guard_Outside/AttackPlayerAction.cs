using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

[System.Serializable, NodeDescription(name: "Attack Player", story: "Attack the player when in range")]
public class AttackPlayerAction : Unity.Behavior.Action
{
    private NavMeshAgent agent;
    private Transform player;
    private SamuraiController playerHealth;

    public float attackRange = 2.0f; // Ab wie viel Meter Abstand angegriffen wird
    public int damage = 10;            // Schaden pro Schlag
    public float attackCooldown = 1.5f; // Zeit zwischen den Angriffen
    private float lastAttackTime;

    protected override Status OnStart()
    {
        // Holt sich den NavMeshAgent direkt und sicher von der Wache, die das Script ausführt
        if (GameObject != null)
        {
            agent = GameObject.GetComponent<NavMeshAgent>();
        }

        // Player über Tag finden
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<SamuraiController>();
        }

        if (agent == null || player == null) return Status.Failure;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (player == null || agent == null) return Status.Failure;

        float distance = Vector3.Distance(agent.transform.position, player.position);

        if (distance <= attackRange)
        {
            // Wache stoppt beim Angriff
            agent.SetDestination(agent.transform.position);

            // Prüfen, ob der Cooldown abgelaufen ist
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
        else
        {
            // Ansonsten weiter hinterherlaufen
            agent.SetDestination(player.position);
        }

        return Status.Running;
    }
}