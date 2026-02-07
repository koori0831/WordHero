using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetColliderActive", story: "[Self] set collider [active]", category: "Action", id: "aa9fc84a660f857639bd532432875488")]
public partial class SetColliderActiveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> Active;

    protected override Status OnStart()
    {
        Collider col = Self.Value.GetComponent<Collider>();
        if (col == null)
            return Status.Failure;

        col.enabled = Active;

        return Status.Success;
    }
}

