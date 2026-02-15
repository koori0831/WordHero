using UnityEngine;

namespace Work.Combat.Code
{
    public struct KnockbackData
    {
        public float Force;
        public float Duration;
        public Vector3 Direction;
        public KnockbackData(float force, float duration, Vector3 direction)
        {
            Force = force;
            Duration = duration;
            Direction = direction;
        }
    }

    public interface IKnockbackable
    {
        void TakeKnockback(KnockbackData knockbackData);
    }
} 
