using System;
using UnityEngine;
using Work.Weapons.HitBox.Code;

namespace Work.Weapons.HitBox.Effects.Code
{
    [Serializable]
    public class SpawnVFXHitEffect : IHitEffect
    {
        public GameObject VFXPrefab;

        public void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint)
        {
            if (VFXPrefab != null)
            {
                GameObject.Instantiate(VFXPrefab, hitPoint, Quaternion.identity);
            }
        }
    }
}
