using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Work.Enemies.Code;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetCanMove", story: "[Self] moveing [setting]", category: "Action", id: "715e81b90c6b649d347c6eee6d77cecb")]
public partial class SetCanMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> Setting;

    protected override Status OnStart()
    {
        EnemyMovementModule mover = Self.Value.GetModule<EnemyMovementModule>();
        Debug.Assert(mover != null, "EnemyMovementModule is missing.");
        mover.SetMovement(Setting.Value);
        return Status.Success;
    }
}

