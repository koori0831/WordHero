using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetRotateFromTransform", story: "Set [Self] rotate [transform]", category: "Action", id: "b1311f2b0952095f855107ce1cc61752")]
public partial class SetRotateFromTransformAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Transform> Transform;

    protected override Status OnStart()
    {
        Self.Value.GetModule<EnemyMovementModule>().SetRotate(Transform.Value.position);
        return Status.Success;
    }
}

