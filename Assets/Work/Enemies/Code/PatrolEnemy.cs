using UnityEngine;

namespace Work.Enemies.Code
{
    public class PatrolEnemy : Enemy
    {
        [SerializeField] protected float patrolRange = 5f;
        [SerializeField] protected int patrolPointCount = 3;

        public override bool VariableSetting()
        {
            if (base.VariableSetting() == false)
                return false;

            SetBlackboardVariable<float>(BTVariables.PatrolRange, patrolRange);
            SetBlackboardVariable<int>(BTVariables.PatrolPointCount, patrolPointCount);
            return true;
        }
    }
}
