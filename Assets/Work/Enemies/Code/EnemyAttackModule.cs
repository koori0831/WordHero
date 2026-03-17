using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Enemies.Code
{
    public class EnemyAttackModule : MonoBehaviour, IAgentModule, IVariableModule
    {
        protected Enemy _owner;

        //아래 두개는 나중에 하나로 묶을예정 지금은 테스트용
        [SerializeField] protected float attackRange;
        public float AttackRange => attackRange;
        [SerializeField] protected int damage;
        [SerializeField] private bool _isComboAttacked;
        [ShowIf(nameof(_isComboAttacked))][SerializeField] protected int attackCount;

        [SerializeField] protected TargetSensor targetSensor;

        //[SerializeField] private 

        public void Initialize(Agent agent)
        {
            _owner = agent as Enemy;
            targetSensor.Init(agent);
        }



        public virtual void Attack()
        {
            List<IDamageable> damageables = targetSensor.Cast<IDamageable>();
            List<IKnockbackable> knockbackables = targetSensor.Cast<IKnockbackable>();

            knockbackables.ForEach(k =>
            {
                k.TakeKnockback(new KnockbackData(5f, 0.5f, (k.Transform.position - _owner.Transform.position).normalized, AnimationCurve.EaseInOut(0, 1, 1, 0)));
            });
            damageables.ForEach(d =>
            {
                DamageContext context = new DamageContext(_owner.gameObject, (d as Component).gameObject, damage);
                DamageResolver.TryApplyDamage(context);
            });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void BTInit()
        {
            if (_isComboAttacked)
                _owner.SetBlackboardVariable<int>(BTVariables.AttackCount, attackCount);
        }
    }
}
