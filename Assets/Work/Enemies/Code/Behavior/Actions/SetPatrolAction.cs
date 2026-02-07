using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPatrol", story: "[Self] set patrol [value]", category: "Action", id: "d8c6df1a703767acb6aed6f4b10f06dc")]
public partial class SetPatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        Self.Value.GetModule<EnemyMovementModule>().SetPatroling(Value.Value);

        return Status.Success;
    }
}

