using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerIsSeen", story: "[Agent] sees [Player] with [Range] Meters and [FieldofView] Degrees", category: "Conditions", id: "1ce6414b61422651ef5ff649ac87f6b6")]
public partial class PlayerIsSeenCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<float> FieldofView;

    public override bool IsTrue()
    {
        //makes sure player and agent variables are assigned to blackboard
       if (Agent.Value == null || Player.Value == null)
            return false;

        //the calcualtions if distance to the player, 
        Vector3 toObject = Player.Value.transform.position - Agent.Value.transform.position;
        if (toObject.magnitude > Range.Value)
            return false;
        //checks if player is in agents (guards) field of view 
        if (Vector3.Angle(Agent.Value.transform.forward, toObject) > FieldofView.Value * 0.5f)
            return false;
        //if ray hits a wall the player is blocked form view
        Vector3 eye = Agent.Value.transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = Player.Value.transform.position + Vector3.up;
        if (Physics.Linecast(eye, targetPoint, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)
            && !hit.transform.IsChildOf(Player.Value.transform))
            return false;
        //if all checks pass then return true
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
