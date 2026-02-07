using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitReachDestination", story: "[Self] wait until you reach your destination", category: "Action", id: "282c0059438fa5f42088d1e509179b24")]
public partial class WaitReachDestinationAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    protected override Status OnStart()
    {
        if (Self.Value.GetModule<EnemyMovementModule>().IsArrived)
        {
            return Status.Success;
        }
        return Status.Failure;
    }
}

