using System;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class SpawnGameObjectEffect : ISkillEffect
    {
        public GameObject Prefab;
        public Vector3 Offset;
        public void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction)
        {
            Vector3 spawnPosition = caster.position + Offset;
            GameObject.Instantiate(Prefab, spawnPosition, Quaternion.identity);
        }
    }
}
