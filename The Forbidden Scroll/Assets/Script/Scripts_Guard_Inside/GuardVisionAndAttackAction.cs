using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using System.Collections.Generic;

[System.Serializable, NodeDescription("Guard Vision and Attack", "Guard patrols or attacks player if in vision")]
public class GuardVisionAndAttackAction : Unity.Behavior.Action
{
    private NavMeshAgent agent;
    private Transform player;
    private NinjaController playerHealth;
    
    private List<Transform> cachedWaypoints = new List<Transform>();
    private int currentWaypointIndex = 0;

    public float movementSpeed = 3.5f;
    public float reachThreshold = 1.5f; 

    // Variable für das Blackboard-Array (Waypoints)
    [SerializeReference]
    public BlackboardVariable<List<GameObject>> Waypoints;

    public float viewDistance = 12.0f; 
    public float viewAngle = 90.0f;    
    public float attackRange = 2.0f;     
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private Vector3 lastPlayerDestination;
    private float lastDestinationUpdateTime;

    protected override Status OnStart()
    {
        if (GameObject != null)
        {
            agent = GameObject.GetComponent<NavMeshAgent>();
        }
        if (agent == null)
        {
            agent = Object.FindAnyObjectByType<NavMeshAgent>();
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<NinjaController>();
        }

        if (agent == null) return Status.Failure;

        // Wegpunkte aus dem Blackboard auslesen
        cachedWaypoints.Clear();
        if (Waypoints != null && Waypoints.Value != null)
        {
            foreach (GameObject obj in Waypoints.Value)
            {
                if (obj != null) cachedWaypoints.Add(obj.transform);
            }
        }

        agent.speed = movementSpeed;
        
        if (cachedWaypoints.Count > 0 && cachedWaypoints[0] != null)
        {
            agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null) return Status.Failure;

        bool playerInVision = false;

        // 1. Sichtfeld-Prüfung zum Spieler
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - agent.transform.position;
            float distance = directionToPlayer.magnitude;

            if (distance <= viewDistance)
            {
                Vector3 forwardWithoutY = new Vector3(agent.transform.forward.x, 0, agent.transform.forward.z).normalized;
                Vector3 directionWithoutY = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
                float angle = Vector3.Angle(forwardWithoutY, directionWithoutY);

                if (angle <= viewAngle / 2f)
                {
                    if (!Physics.Linecast(agent.transform.position + Vector3.up * 1.0f, player.position + Vector3.up * 1.0f))
                    {
                        playerInVision = true;

                        if (distance <= attackRange)
                        {
                            agent.SetDestination(agent.transform.position); 
                            if (Time.time >= lastAttackTime + attackCooldown)
                            {
                                lastAttackTime = Time.time;
                                if (playerHealth != null) playerHealth.TakeDamage(damage);
                            }
                        }
                        else
                        {
                            if (Time.time > lastDestinationUpdateTime + 0.3f || Vector3.Distance(player.position, lastPlayerDestination) > 1.0f)
                            {
                                agent.SetDestination(player.position);
                                lastPlayerDestination = player.position;
                                lastDestinationUpdateTime = Time.time;
                            }
                        }
                    }
                }
            }
        }

        // 2. Patrouillieren, wenn Spieler nicht im Sichtfeld
        if (!playerInVision)
        {
            if (cachedWaypoints != null && cachedWaypoints.Count > 0)
            {
                agent.speed = movementSpeed;

                if (agent.hasPath && agent.remainingDistance <= reachThreshold)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % cachedWaypoints.Count;
                    if (cachedWaypoints[currentWaypointIndex] != null)
                    {
                        agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
                    }
                }
                else if (!agent.hasPath && !agent.pathPending)
                {
                    if (cachedWaypoints[currentWaypointIndex] != null)
                    {
                        agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
                    }
                }
            }
        }

        return Status.Running;
    }
}