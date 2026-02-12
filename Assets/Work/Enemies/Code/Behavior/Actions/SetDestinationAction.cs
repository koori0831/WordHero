using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetDestination", story: "[Self] set [destination]", category: "Action", id: "071f8ad42cb5ccd88505e48fdaea5848")]
public partial class SetDestinationAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Vector3> Destination;

    protected override Status OnStart()
    {
        Self.Value.GetModule<EnemyMovementModule>().SetDestination(Destination);
        return Status.Success;
    }
}

