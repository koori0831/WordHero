using System.Collections.Generic;
using UnityEngine;
using Work.Combat.Code;
using Work.Entities;

namespace Work.Enemies.Code
{
    public class EnemyAttackModule : MonoBehaviour, IEnemyModule
    {
        private Enemy _owner;

        //아래 두개는 나중에 하나로 묶을예정 지금은 테스트용
        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;
        [SerializeField] private int damage;

        [SerializeField] private TargetSensor targetSensor;

        //[SerializeField] private 

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
        }

        public void Attack()
        {
            List<IDamageable> damageables = targetSensor.Cast();
            damageables.ForEach(d => d.TakeDamage(damage));

            //공격로직 구현
            Debug.Log($"{_owner.name} attacks with {damage} damage within {attackRange} range.");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

    }
}