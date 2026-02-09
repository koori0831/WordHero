using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAutoMove", story: "[Self] set auto move [value]", category: "Action", id: "74c7ba9243512318d6a814480f6df5cb")]
public partial class SetAutoMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> Value;

    protected override Status OnStart()
    {
        Self.Value.GetModule<EnemyMovementModule>().SetAutoMove(Value.Value);
        return Status.Success;
    }

}

