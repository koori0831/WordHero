using Code.Entities;
using Work.Core.Utils.EventBus;

namespace Work.StatSystem.Code
{
    public readonly record struct StatApplyModifierEvent(Entity Target, StatSO Stat, object Key, StatModifierSpec Spec) : IEvent;
    public readonly record struct StatRemoveModifierEvent(Entity Target, StatSO Stat, object Key) : IEvent;
}