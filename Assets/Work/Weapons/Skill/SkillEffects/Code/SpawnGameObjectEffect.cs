using Cysharp.Threading.Tasks;
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
        public bool SetParentToCaster = false;
        public float Delay = 0f;

        public void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction)
        {
            Spawn(caster);
        }

        private async UniTaskVoid Spawn(Transform caster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Delay));

            Vector3 spawnPosition = caster.TransformPoint(Offset);
            Quaternion prefabRotation = Prefab.transform.rotation;
            Quaternion spawnRotation = caster.rotation * prefabRotation;

            if (SetParentToCaster)
                GameObject.Instantiate(Prefab, spawnPosition, spawnRotation, caster);
            else
                GameObject.Instantiate(Prefab, spawnPosition, spawnRotation);
        }
    }
}
