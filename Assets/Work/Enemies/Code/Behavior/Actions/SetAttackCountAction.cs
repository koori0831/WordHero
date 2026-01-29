using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAttackCount", story: "[Self] set attack animation count random [range]", category: "Action", id: "2d57ef7ba3f330cc409cf2af1ce7f8f3")]
public partial class SetAttackCountAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<int> Range;

    protected override Status OnStart()
    {
        EnemyAnimatorModule animator = Self.Value.GetModule<EnemyAnimatorModule>();
        Debug.Assert(animator != null, "Animator module not found in enemy.");

        int attackCount = UnityEngine.Random.Range(0, Range.Value);
        animator.SetParam(Animator.StringToHash("ATTACK_COUNT"),(float)attackCount);
        return Status.Running;
    }
}

