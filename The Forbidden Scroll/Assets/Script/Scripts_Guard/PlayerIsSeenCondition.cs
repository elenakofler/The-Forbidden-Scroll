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
       if (Agent.Value == null || Player.Value == null)
            return false;

        Vector3 toObject = Player.Value.transform.position - Agent.Value.transform.position;
        if (toObject.magnitude > Range.Value)
            return false;

        if (Vector3.Angle(Agent.Value.transform.forward, toObject) > FieldofView.Value * 0.5f)
            return false;

        Vector3 eye = Agent.Value.transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = Player.Value.transform.position + Vector3.up;
        if (Physics.Linecast(eye, targetPoint, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore)
            && !hit.transform.IsChildOf(Player.Value.transform))
            return false;

        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
