using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "[Self] Detect [target] with in [TargetLayer] detection range [N]", category: "Action", id: "688d90d758b7eff9836a665bc8fb74d0")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<int> TargetLayer;
    [SerializeReference] public BlackboardVariable<float> N;

    protected override Status OnUpdate()
    {
        if (Target.Value != null)
            return Status.Success;

        Collider[] col = Physics.OverlapSphere(Self.Value.Transform.position, N.Value, TargetLayer.Value);

        if (col.Length >= 1)
        {
            Target.Value = col[0].transform;
            Self.Value.GetModule<EnemyMovementModule>().SetTarget(Target.Value);
            return Status.Success;
        }

        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

