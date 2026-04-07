using System;
using Work.Weapons.Imprint.Code;

namespace Work.Weapons.Code
{
    [Serializable]
    public class WeaponRuntimeInstance
    {
        public BaseWeapon Weapon;
        public WeaponImprintSlots Imprints = new();
        public WeaponTriggerRuntime TriggerRuntime = new();

        public WeaponRuntimeInstance(BaseWeapon weapon)
        {
            Weapon = weapon;
        }
    }
}
