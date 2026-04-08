using System;
using UnityEngine;
using Work.Weapons.Code;
using Work.Weapons.HitBox.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class DestroyGameObjectWithDelay : IHitEffect
    {
        public float delay;
        public GameObject gameObjcet;

        public void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint)
        {
            MonoBehaviour.Destroy(gameObjcet, delay);
        }
    }
}
