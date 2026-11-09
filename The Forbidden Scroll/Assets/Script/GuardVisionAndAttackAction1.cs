using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Guard Vision and Attack", story: "Guard patrols or attacks [player] if in vision", category: "Action", id: "88819118fadfbddffeff5e4d32ae210b")]
public partial class GuardVisionAndAttackAction1 : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

