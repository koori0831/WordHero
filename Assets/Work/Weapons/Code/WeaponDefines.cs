using UnityEngine;

namespace Work.Weapons.Code
{
    public interface ISkillEffect
    {
        void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction);
    }

    public enum WeaponType
    {
        Melee,
        Ranged,
        Magic
    }
}