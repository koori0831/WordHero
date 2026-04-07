using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Code;
using Work.Weapons.HitBox.Code;
using Work.Weapons.Skill.Code;

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

        public void ExecuteEffect(SkillContext context)
        {
            Spawn(context.Caster).Forget();
        }

        private async UniTaskVoid Spawn(Player caster)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Delay));

            Vector3 spawnPosition = caster.transform.TransformPoint(Offset);
            Quaternion prefabRotation = Prefab.transform.rotation;
            Quaternion spawnRotation = caster.transform.rotation * prefabRotation;
            Transform parent = SetParentToCaster ? caster.transform : null;

            GameObject temp = GameObject.Instantiate(Prefab, spawnPosition, spawnRotation, parent);

            if (temp != null)
            {
                if (temp.TryGetComponent(out GenericHitbox hitbox))
                {
                    hitbox.Owner = caster.gameObject;
                }

                if (UseScaleOverride)
                {
                    temp.transform.localScale = ScaleOverride;
                }
            }
        }
    }
}
