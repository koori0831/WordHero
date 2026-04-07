using System;
using UnityEngine;

namespace Work.Weapons.Imprint.Code
{
    [Serializable]
    public class WeaponTriggerRuntime
    {
        [SerializeField] private bool isTriggered;
        [SerializeField] private float expireTime;

        public bool IsTriggered => isTriggered;
        public float ExpireTime => expireTime;

        public void Open(float duration)
        {
            isTriggered = true;
            expireTime = Time.time + duration;
        }

        public bool CanActivate()
        {
            return isTriggered && Time.time <= expireTime;
        }

        public void Consume()
        {
            isTriggered = false;
            expireTime = 0f;
        }

        public void Update()
        {
            if (!isTriggered)
                return;

            if (Time.time > expireTime)
                Consume();
        }
    }
}
