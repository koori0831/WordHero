using System;
using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class CameraImpulseEffect : ISkillEffect
    {
        [SerializeField] private float impulseForce = 1f;
        [SerializeField] private float impulseDuration = 0.5f;

        public void ExecuteEffect(SkillContext context)
        {
            CameraController.Instance.PlayImpulse(impulseForce, impulseDuration);
        }
    }
}
