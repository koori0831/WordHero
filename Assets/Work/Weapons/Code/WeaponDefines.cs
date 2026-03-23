using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Players.Code;

namespace Work.Weapons.Code
{
    public interface ISkillEffect
    {
        void ExecuteEffect(Player caster, Vector3 target, Vector3 direction);
    }

    public enum WeaponType
    {
        Melee,
        Ranged,
        Magic
    }

    public readonly record struct SkillMotionEndEvent() : IEvent;
}