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
    
    public float attackRange = 2.0f;
    public int damage = 10;
    public float attackCooldown = 1.5f;
    
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

        // If out of range, fail so the tree can switch back to a chase/navigate behavior
        if (distance > attackRange)
        {
            m_Agent.isStopped = false;
            return Status.Failure; 
        }

        // In range: Stop and attack
        m_Agent.isStopped = true;

        if (Time.time >= m_LastAttackTime + attackCooldown)
        {
            m_LastAttackTime = Time.time;
            
            if (Target.Value.TryGetComponent<SamuraiController>(out var playerHealth))
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