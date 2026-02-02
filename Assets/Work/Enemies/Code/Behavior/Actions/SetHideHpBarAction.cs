using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetHideHPBar", story: "[Self] decide whether to hide the HP bar or [not]", category: "Action", id: "2b882ffca8c73e5cdb3024b070c4ed40")]
public partial class SetHideHpBarAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<bool> Not;

    protected override Status OnStart()
    {
        EnemyHPBarModule bar = Self.Value.GetModule<EnemyHPBarModule>();
        Debug.Assert(bar != null, "EnemyHPBarModule is null");
        bar.SetActiveBar(Not.Value);
        return Status.Running;
    }
}

