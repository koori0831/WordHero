using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitReachDestination", story: "[Self] wait until you reach your destination", category: "Action", id: "282c0059438fa5f42088d1e509179b24")]
public partial class WaitReachDestinationAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> IsRunning;

    private EnemyMovementModule _mover;

    protected override Status OnStart()
    {
        _mover = Self.Value.GetModule<EnemyMovementModule>();

        if (_mover.IsArrived)
        {
            return Status.Success;
        }

        if (IsRunning)
            return Status.Running;

        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        if (_mover.IsArrived)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}

