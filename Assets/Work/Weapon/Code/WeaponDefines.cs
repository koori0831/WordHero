using UnityEngine;

namespace Work.Weapon.Code
{
    public interface ISkillEffect
    {
        void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction);
    }
}