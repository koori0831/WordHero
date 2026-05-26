using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Work.Enemies.Code.AttackModules;

[Serializable, GeneratePropertyBag]
[Condition(name: "ChargeAttackShouldMelee", story: "[Self] charge attack should melee", category: "Conditions", id: "d6a911fcb3be4bf6a46b6079161724aa")]
public partial class ChargeAttackShouldMeleeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    public override bool IsTrue()
    {
        ChargeAttackModule chargeAttackModule = Self.Value.GetModule<ChargeAttackModule>(true);
        return chargeAttackModule != null && chargeAttackModule.IsTargetInMeleeTransitionDistance;
    }
}
