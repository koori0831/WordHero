using UnityEngine;

namespace Work.Combat.Code
{
    public readonly struct DamageContext
    {
        public GameObject Source { get; }
        public GameObject Target { get; }
        public int Damage { get; }
        public bool IsCritical { get; }

        public DamageContext(GameObject source, GameObject target, int damage, bool isCritical = false)
        {
            Source = source;
            Target = target;
            Damage = damage;
            IsCritical = isCritical;
        }
    }
}
