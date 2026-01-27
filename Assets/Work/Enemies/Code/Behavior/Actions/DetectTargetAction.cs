using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectTarget", story: "Detect [targets] with in detection range [N]", category: "Action", id: "688d90d758b7eff9836a665bc8fb74d0")]
public partial class DetectTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Targets;
    [SerializeReference] public BlackboardVariable<float> N;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

