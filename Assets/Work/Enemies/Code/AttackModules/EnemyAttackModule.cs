using Alchemy.Inspector;
using UnityEngine;
using Work.Agents.Code;

namespace Work.Enemies.Code.AttackModules
{
    public class EnemyAttackModule : MonoBehaviour, IAgentModule, IVariableModule
    {
        protected Enemy _owner;

        [SerializeField] protected float attackRange;
        [SerializeField] protected int damage;
        [SerializeField] private bool _isComboAttacked;

        [ShowIf(nameof(_isComboAttacked))]
        [SerializeField] protected int attackCount;

        public float AttackRange => attackRange;

        public virtual void Initialize(Agent agent)
        {
            _owner = agent as Enemy;
        }

        public virtual void Attack()
        {
        }

        public virtual void BTInit()
        {
            if (_isComboAttacked)
                _owner.SetBlackboardVariable<int>(BTVariables.AttackCount, attackCount);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
