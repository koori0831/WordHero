using UnityEngine;

namespace Work.Combat.Code
{
    public struct KnockbackData
    {
        public float Force;
        public float Duration;
        public Vector3 Direction;
        public AnimationCurve KnockbackCurve; // 넉백의 힘이 시간에 따라 어떻게 변화하는지 정의하는 커브

        public KnockbackData(float force, float duration, Vector3 direction, AnimationCurve animCurve)
        {
            Force = force;
            Duration = duration;
            Direction = direction;
            KnockbackCurve = animCurve;
        }
    }

    public interface IKnockbackable
    {
        void TakeKnockback(KnockbackData knockbackData);
    }
} 
