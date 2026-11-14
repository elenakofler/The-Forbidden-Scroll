using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using System.Collections.Generic;
using System.Linq;

[System.Serializable, NodeDescription("Guard Vision and Attack", "Guard patrols or attacks player if in vision")]
public class GuardVisionAndAttackAction : Unity.Behavior.Action
{
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Transform player;
    private NinjaController playerHealth;
    
    private List<Transform> cachedWaypoints = new List<Transform>();
    private int currentWaypointIndex = 0;

    public float movementSpeed = 3.5f;
    public float reachThreshold = 2.0f; // Etwas erhöht, damit der Übergang butterweich ist

    [SerializeReference]
    public BlackboardVariable<GameObject> Player;

    public float viewDistance = 12.0f; 
    public float viewAngle = 90.0f;    
    public float attackRange = 2.0f;     
    public int damage = 10;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private Vector3 lastPlayerDestination;
    private float lastDestinationUpdateTime;
    private bool wasPlayerInVisionLastFrame = false;

    protected override Status OnStart()
    {
        if (GameObject != null)
        {
            agent = GameObject.GetComponent<NavMeshAgent>();
            rb = GameObject.GetComponent<Rigidbody>();
        }
        if (agent == null)
        {
            agent = Object.FindAnyObjectByType<NavMeshAgent>();
        }

        if (rb != null) rb.isKinematic = true;

        // Spieler finden
        GameObject playerObj = null;
        if (Player != null && Player.Value != null)
        {
            playerObj = Player.Value;
        }
        else
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) playerObj = GameObject.Find("Ninja");
        }

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<NinjaController>();
        }

        if (agent == null) return Status.Failure;

        // Wegpunkte automatisch im Parent finden
        cachedWaypoints.Clear();
        if (agent.transform.parent != null)
        {
            cachedWaypoints = agent.transform.parent
                .Cast<Transform>()
                .Where(t => t.name.StartsWith("Point"))
                .OrderBy(t => t.name)
                .ToList();
        }

        agent.speed = movementSpeed;
        agent.isStopped = false;
        
        if (cachedWaypoints.Count > 0 && cachedWaypoints[0] != null)
        {
            agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
        }

        wasPlayerInVisionLastFrame = false;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null || !agent.isOnNavMesh) return Status.Running;

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
                    Vector3 rayStart = agent.transform.position + Vector3.up * 1.0f;
                    RaycastHit hit;

                    if (Physics.Raycast(rayStart, directionToPlayer.normalized, out hit, viewDistance))
                    {
                        if (hit.transform == player || hit.transform.IsChildOf(player))
                        {
                            playerInVision = true;

                            agent.speed = movementSpeed;
                            agent.isStopped = false;

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
                                // Verfolgung ohne Ruckeln
                                if (Time.time > lastDestinationUpdateTime + 0.05f || Vector3.Distance(player.position, lastPlayerDestination) > 0.1f)
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
        }

        // --- ÜBERGANG: Zurück zur Patrouille ---
        if (wasPlayerInVisionLastFrame && !playerInVision)
        {
            agent.isStopped = false;
            if (cachedWaypoints.Count > 0 && cachedWaypoints[currentWaypointIndex] != null)
            {
                agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
            }
        }
        wasPlayerInVisionLastFrame = playerInVision;

        // 2. Patrouillieren
        if (!playerInVision)
        {
            if (cachedWaypoints != null && cachedWaypoints.Count > 0)
            {
                agent.speed = movementSpeed;
                agent.isStopped = false;

                Transform targetPoint = cachedWaypoints[currentWaypointIndex];
                if (targetPoint != null)
                {
                    float distanceToWaypoint = Vector3.Distance(
                        new Vector3(agent.transform.position.x, 0, agent.transform.position.z), 
                        new Vector3(targetPoint.position.x, 0, targetPoint.position.z)
                    );

                    // Wechselt zum nächsten Wegpunkt, sobald er nah genug ist
                    if (distanceToWaypoint <= reachThreshold || (!agent.hasPath && !agent.pathPending))
                    {
                        currentWaypointIndex = (currentWaypointIndex + 1) % cachedWaypoints.Count;
                        if (cachedWaypoints[currentWaypointIndex] != null)
                        {
                            agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
                        }
                    }
                }
            }
        }

        return Status.Running;
    }
}