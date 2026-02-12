using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public readonly record struct CombatHitEvent(GameObject Source, GameObject Target, int Damage, bool IsCritical) : IEvent;
}

