using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;
using Unity.Properties;

[System.Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack Player", story: "[Agent] attacks [Target]")]
public class AttackPlayerAction : Unity.Behavior.Action
{
    [SerializeReference] public BlackboardVariable<GameObject> AgentGameObject;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    
    public float attackRange = 4.0f;
    public int damage = 10;
    public float attackCooldown = 1f;
    
    private NavMeshAgent m_Agent;
    private float m_LastAttackTime;

    protected override Status OnStart()
    {
        if (AgentGameObject?.Value == null || Target?.Value == null)
            return Status.Failure;

        m_Agent = AgentGameObject.Value.GetComponent<NavMeshAgent>();
        if (m_Agent == null) return Status.Failure;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null || m_Agent == null) 
            return Status.Failure;

        float distance = Vector3.Distance(m_Agent.transform.position, Target.Value.transform.position);

        // If out of range, chase the player!
        if (distance > attackRange)
        {
            m_Agent.isStopped = false;
            
            // Tell the agent to walk to the player's current position
            m_Agent.SetDestination(Target.Value.transform.position);
            
            // Return Running so the node stays active while chasing
            return Status.Running; 
        }

        // In range: Stop and attack
        m_Agent.isStopped = true;

        if (Time.time >= m_LastAttackTime + attackCooldown)
        {
            m_LastAttackTime = Time.time;
            
            if (Target.Value.TryGetComponent<NinjaController>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (m_Agent != null)
        {
            m_Agent.isStopped = false;
        }
    }
}