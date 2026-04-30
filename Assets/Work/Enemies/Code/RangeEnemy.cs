using UnityEngine;

namespace Work.Enemies.Code
{
    public class RangeEnemy : Enemy
    {
        [SerializeField] private float moveDistance;
        [SerializeField] private float distanceToKeepRange;

        public override bool VariableSetting()
        {
            if (base.VariableSetting() == false)
                return false;

            SetBlackboardVariable<float>(BTVariables.MoveDistance, moveDistance);
            SetBlackboardVariable<float>(BTVariables.DistanceToKeepRange, distanceToKeepRange);
            return true;
        }
    }
}
