using UnityEngine;

namespace Work.Enemies.Code
{
    public class PatrolEnemy : Enemy
    {
        [SerializeField] protected float patrolRange = 5f;
        [SerializeField] protected int patrolPointCount = 3;

        public override void VariableSetting()
        {
            base.VariableSetting();
            SetBlackboardVariable<float>(BTVariables.PatrolRange, patrolRange);
            SetBlackboardVariable<int>(BTVariables.PatrolPointCount, patrolPointCount);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, patrolRange);
        }
    }
}