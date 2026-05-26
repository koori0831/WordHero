using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Work.Enemies.Code.AttackModules;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetChargeAttackPhaseByDistance", story: "[Self] set charge attack phase by distance to [ParameterName]", category: "Action", id: "63fb46924ff44770a59d71830aa40f34")]
public partial class SetChargeAttackPhaseByDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<string> ParameterName;

    protected override Status OnStart()
    {
        EnemyAnimatorModule animator = Self.Value.GetModule<EnemyAnimatorModule>();
        ChargeAttackModule chargeAttackModule = Self.Value.GetModule<ChargeAttackModule>(true);
        Debug.Assert(animator != null, "EnemyAnimatorModule is missing.");
        Debug.Assert(chargeAttackModule != null, "ChargeAttackModule is missing.");

        string parameterName = string.IsNullOrEmpty(ParameterName.Value) ? "ATTACK_PHASE" : ParameterName.Value;
        int phase = chargeAttackModule.IsTargetInMeleeTransitionDistance ? 1 : 0;
        animator.SetParam(Animator.StringToHash(parameterName), phase);
        return Status.Success;
    }
}
