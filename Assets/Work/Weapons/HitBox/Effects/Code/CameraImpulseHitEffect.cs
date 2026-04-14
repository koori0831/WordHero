using System;
using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Weapons.HitBox.Code;

namespace Work.Weapons.HitBox.Effects.Code
{
    [Serializable]
    public class CameraImpulseHitEffect : IHitEffect
    {
        [SerializeField] private float impulseForce = 1f;
        [SerializeField] private float impulseDuration = 0.5f;

        public void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint)
        {
            CameraController.Instance.PlayImpulse(impulseForce, impulseDuration);
        }
    }
}