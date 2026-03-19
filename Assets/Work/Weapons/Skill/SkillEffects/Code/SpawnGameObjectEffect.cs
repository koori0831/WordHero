using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Alchemy;
using Work.Weapons.Code;
using Alchemy.Inspector;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [Serializable]
    public class SpawnGameObjectEffect : ISkillEffect
    {
        public GameObject Prefab;
        public Vector3 Offset;
        public bool UseScaleOverride = false;
        [ShowIf(nameof(UseScaleOverride))]
        public Vector3 ScaleOverride = Vector3.one;
        public bool SetParentToCaster = false;
        public float Delay = 0f;

        public void ExecuteEffect(Transform caster, Vector3 target, Vector3 direction)
        {
            Spawn(caster).Forget(); 
        }

        private async UniTaskVoid Spawn(Transform caster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Delay));

            Vector3 spawnPosition = caster.TransformPoint(Offset);
            Quaternion prefabRotation = Prefab.transform.rotation;
            Quaternion spawnRotation = caster.rotation * prefabRotation;

            if (SetParentToCaster)
            {
                GameObject temp = GameObject.Instantiate(Prefab, spawnPosition, spawnRotation, caster);
                if (temp != null && UseScaleOverride)
                {
                    temp.transform.localScale = ScaleOverride;
                }
            }
            else
            {
                GameObject temp = GameObject.Instantiate(Prefab, spawnPosition, spawnRotation);
                if (temp != null && UseScaleOverride)
                {
                    temp.transform.localScale = ScaleOverride;
                }
            }
        }
    }
}
