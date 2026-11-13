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
    
    private Transform[] cachedWaypoints;
    private int currentWaypointIndex = 0;

    public float movementSpeed = 3.5f;
    public float reachThreshold = 1.5f; 
    
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
        agent = Object.FindAnyObjectByType<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<NinjaController>();
        }

        if (agent == null) return Status.Failure;

        // Waypoints in der Szene suchen
        List<Transform> foundWaypoints = new List<Transform>();
        GameObject pointA = GameObject.Find("PointA");
        GameObject pointB = GameObject.Find("PointB");
        
        if (pointA != null) foundWaypoints.Add(pointA.transform);
        if (pointB != null) foundWaypoints.Add(pointB.transform);
        cachedWaypoints = foundWaypoints.ToArray();

        agent.speed = movementSpeed;
        
        if (cachedWaypoints.Length > 0 && cachedWaypoints[0] != null)
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

                        // Spieler im Sichtfeld -> Angriff oder Verfolgung
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
                            // Ziel nur aktualisieren, wenn sich der Spieler bewegt hat oder Zeit vergangen ist (verhindert das Drehen!)
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

        // 2. Wenn Spieler NICHT im Sichtfeld -> Patrouillieren
        if (!playerInVision)
        {
            if (cachedWaypoints != null && cachedWaypoints.Length > 0)
            {
                agent.speed = movementSpeed;

                // Prüfen, ob der Wegpunkt erreicht wurde
                if (agent.hasPath && agent.remainingDistance <= reachThreshold)
                {
                    currentWaypointIndex = (currentWaypointIndex + 1) % cachedWaypoints.Length;
                    if (cachedWaypoints[currentWaypointIndex] != null)
                    {
                        agent.SetDestination(cachedWaypoints[currentWaypointIndex].position);
                    }
                }
                else if (!agent.hasPath && !agent.pathPending)
                {
                    // Falls kein Pfad aktiv ist, direkt zum aktuellen Wegpunkt setzen
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