using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Combat.Code
{
    public class DamageTextEvents { }

    public struct DamageTextEvent : IEvent
    {
        public int Damage { get; }
        public bool IsCritical { get; }
        public GameObject Owner { get; }
        public DamageTextType DamageTextType { get; }

        public DamageTextEvent(int damage, GameObject owner, bool isCritical, DamageTextType damageTextType = DamageTextType.Normal)
        {
            Damage = damage;
            IsCritical = isCritical;
            Owner = owner;
            DamageTextType = damageTextType;
        }
    }
}
