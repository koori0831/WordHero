using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public readonly record struct CombatHitEvent(GameObject Source, GameObject Target, int Damage, bool IsCritical) : IEvent;
    public readonly record struct CombatDodgeEvent(GameObject Source) : IEvent;
    public readonly record struct GetSkillEnergyEvent(float amount) : IEvent;
}

