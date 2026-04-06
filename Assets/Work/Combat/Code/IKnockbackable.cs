using UnityEngine;

namespace Work.Combat.Code
{
    public struct KnockbackData
    {
        public float Force;
        public float Duration;
        public float StiffDuration; // 넉백으로 인한 경직 시간
        public Vector3 Direction;
        public AnimationCurve KnockbackCurve; // 넉백의 힘이 시간에 따라 어떻게 변화하는지 정의하는 커브

        public KnockbackData(float force, float duration, Vector3 direction, AnimationCurve animCurve, float stiffDuration = 0.2f)
        {
            Force = force;
            Duration = duration;
            Direction = direction;
            KnockbackCurve = animCurve;
            StiffDuration = stiffDuration;
        }
    }

    public interface IKnockbackable : ICastable
    {
        Transform Transform { get; }
        void TakeKnockback(KnockbackData knockbackData);
    }
} 
