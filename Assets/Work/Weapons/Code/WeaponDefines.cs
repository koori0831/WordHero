using System;
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

    [Serializable]
    public record struct ComboHitBox
    {
        public GameObject HitBoxPrefab;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
    }

    public readonly record struct SkillMotionEndEvent() : IEvent;
}