using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetNavAgentActive", story: "[Self] set navmesh agent [active]", category: "Action", id: "5618d2f5359a5ee2b075cb3b75dc67c3")]
public partial class SetNavAgentActiveAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> Active;

    protected override Status OnStart()
    {
        Self.Value.NavAgent.enabled = Active.Value;
        return Status.Success;
    }
}

