using System;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Players.Code;

namespace Work.Weapons.Code
{
    public interface ISkillEffect
    {
        void ExecuteEffect(SkillContext context);
    }

    public interface IImprintTriggerEvent : IEvent
    {
        Action Subscribe(Action onTriggered);
    }

    public enum WeaponType
    {
        OneHandSword,
        TwoHandSword,
        Axe,
        Polearm,
        Blunt
    }

    [Serializable]
    public record struct ComboHitBox
    {
        public GameObject HitBoxPrefab;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public bool SetParentToCaster;
    }

    public enum ImprintType
    {
        Attack,
        Effect,
        Stat
    }

    public readonly record struct SkillMotionEndEvent() : IEvent;
    public record SkillContext(Player Caster, Vector3 Target, Vector3 Direction);
}
