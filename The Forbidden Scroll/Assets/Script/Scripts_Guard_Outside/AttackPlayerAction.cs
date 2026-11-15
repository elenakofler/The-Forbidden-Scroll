using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

[System.Serializable, NodeDescription(name: "Attack Player", story: "Attack the player when in range")]
public class AttackPlayerAction : Unity.Behavior.Action
{
    private NavMeshAgent agent;
    private Transform player;
    private SamuraiController playerHealth;

    public float attackRange = 2.0f; //at what distance the attack occures
    public int damage = 10;            //Damage per hit
    public float attackCooldown = 1.5f; //Time between attacks
    private float lastAttackTime;

    protected override Status OnStart()
    {
        //Getting the NavMeshAgent directly from the guard
        if (GameObject != null)
        {
            agent = GameObject.GetComponent<NavMeshAgent>();
        }

        //find player with tag
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
            //Guard stops when attacking
            agent.SetDestination(agent.transform.position);

            //Checking, if the cooldown is over
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
            //Otherwise, keep chasing
            agent.SetDestination(player.position);
        }

        return Status.Running;
    }
}