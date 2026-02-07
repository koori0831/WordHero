using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetMoveSpeed", story: "[Self] set move speed [value]", category: "Action", id: "767582996973d22f99306b3251661f1b")]
public partial class SetMoveSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<float> Value;

    protected override Status OnStart()
    {
        Self.Value.GetModule<EnemyMovementModule>().SetSpeed(Value);
        return Status.Success;
    }
}

