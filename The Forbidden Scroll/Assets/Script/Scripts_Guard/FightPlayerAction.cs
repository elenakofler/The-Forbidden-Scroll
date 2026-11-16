using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FightPlayer", story: "[Agent] fight [Player]", category: "Action", id: "bcace8d9a5aff529a7eb710c18cd9940")]
public partial class FightPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    public float attackRange = 4.0f;
    public int damage = 10;
    public float attackCooldown = 1f;

    private NavMeshAgent m_Agent;
    private float m_LastAttackTime = -999f; // Replaced static dictionary with a clean instance variable

    protected override Status OnStart()
    {
        if (Agent?.Value == null || Player?.Value == null)
            return Status.Failure;

        m_Agent = Agent.Value.GetComponent<NavMeshAgent>();
        if (m_Agent == null) return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Player.Value == null || m_Agent == null) 
            return Status.Failure;

        float distance = Vector3.Distance(m_Agent.transform.position, Player.Value.transform.position);

        // If out of range, chase the target!
        if (distance > attackRange)
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(Player.Value.transform.position);
            return Status.Running; 
        }

        // In range: Stop and attack
        m_Agent.isStopped = true;

        // Handle cooldown per agent instance safely
        if (Time.time >= m_LastAttackTime + attackCooldown)
        {
            m_LastAttackTime = Time.time;
            
            if (Player.Value.TryGetComponent<NinjaController>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        // Safely reset isStopped using our cached m_Agent reference
        if (m_Agent != null && m_Agent.enabled && m_Agent.isOnNavMesh)
        {
            m_Agent.isStopped = false;
        }
    }
}

