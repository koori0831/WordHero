using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Work.Weapons.HitBox.Code
{
    public interface IHitEffect
    {
        // caster: 스킬을 처음 시전한 주체 (플레이어)
        // target: 맞은 대상 (적)
        // hitPoint: 충돌 발생 위치
        void ExecuteHit(GameObject caster, GameObject target, Vector3 hitPoint);
    }
}
