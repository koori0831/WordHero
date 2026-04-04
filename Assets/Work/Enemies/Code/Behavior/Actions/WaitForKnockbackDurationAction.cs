using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Work.Agents.Code;
using Work.Combat.Code;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForKnockbackDuration", story: "[Self] wait for knockback duration", category: "Action", id: "2a2456f2538ef4d06ee1334037447156")]
public partial class WaitForKnockbackDurationAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    private float _waitTime;

    protected override Status OnStart()
    {
        KnockbackData data = Self.Value.GetModule<AgentKnockbackModule>().LastKnockbackData;
        _waitTime = data.Duration;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _waitTime -= Time.deltaTime;
        if (_waitTime <= 0)
        {
            return Status.Success;
        }
        Debug.Log(_waitTime);
        return Status.Running;
    }
}

