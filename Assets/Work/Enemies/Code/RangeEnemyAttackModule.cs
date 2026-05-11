using UnityEngine;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class RangeEnemyAttackModule : EnemyAttackModule
    {
        [SerializeField] private Projectile arrowPrefab;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float arrowSpeed;

        public override void Attack()
        {
            Projectile arrow =  Instantiate(arrowPrefab, muzzle.position,Quaternion.identity);
            arrow.Init(damage,arrowSpeed,_owner.transform.forward, _owner.gameObject);
        }
    }
}
