using UnityEngine;

namespace Work.Weapons.Code
{
    public class MeleeWeapon : BaseWeapon
    {
        public override void Attack(int comboCount = 0)
        {
            base.Attack(comboCount);
            if (Data?.ComboHitBoxes == null || Data.ComboHitBoxes.Count <= 0) return;
            if (comboCount >= Data.ComboHitBoxes.Count) return;

            ComboHitBox hitBox = Data.ComboHitBoxes[comboCount];

            Vector3 spawnPosition = Owner.transform.TransformPoint(hitBox.LocalPosition);
            Quaternion prefabRotation = Quaternion.Euler(hitBox.LocalRotation);
            Quaternion spawnRotation = Owner.transform.rotation * prefabRotation;

            GameObject hitBoxInstance = Instantiate(hitBox.HitBoxPrefab, spawnPosition, spawnRotation);
            if (hitBox.SetParentToCaster)
                hitBoxInstance.transform.SetParent(Owner.transform);
        }
    }
}
