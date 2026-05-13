using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetRotateFromTransform", story: "Set [Self] rotate [transform] [IsSmooth]", category: "Action", id: "b1311f2b0952095f855107ce1cc61752")]
public partial class SetRotateFromTransformAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Transform> Transform;
    [SerializeReference] public BlackboardVariable<bool> IsSmooth;
    private EnemyMovementModule _mover;

    protected override Status OnStart()
    {
        _mover = Self.Value.GetModule<EnemyMovementModule>();

        if (Transform.Value == null || _mover == null)
            return Status.Failure;
        
        if(IsSmooth.Value)
        {
            _mover.SetRotate(Transform.Value.position, IsSmooth.Value);
            return Status.Success;
        }
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_mover.SetRotate(Transform.Value.position, IsSmooth.Value)) return Status.Running; ;
        return Status.Success;
    }
}

