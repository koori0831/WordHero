using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAttackCount", story: "[Self] set attack animation count random [range]", category: "Action", id: "2d57ef7ba3f330cc409cf2af1ce7f8f3")]
public partial class SetAttackCountAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<int> Range;

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

