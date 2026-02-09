using System.Collections.Generic;
using UnityEngine;
using Work.Combat.Code;
using Work.Entities;

namespace Work.Enemies.Code
{
    public class EnemyAttackModule : MonoBehaviour, IEnemyModule
    {
        protected Enemy _owner;

        //아래 두개는 나중에 하나로 묶을예정 지금은 테스트용
        [SerializeField] protected float attackRange;
        public float AttackRange => attackRange;
        [SerializeField] protected int damage;

        [SerializeField] protected TargetSensor targetSensor;

        //[SerializeField] private 

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            targetSensor.Init(enemy);
        }



        public virtual void Attack()
        {
            List<IDamageable> damageables = targetSensor.Cast();
            damageables.ForEach(d => d.TakeDamage(damage));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

    }
}