using UnityEngine;

namespace Work.Enemies.Code
{
    public class RangeEnemy : Enemy
    {
        [SerializeField] private float moveDistance;
        [SerializeField] private float distanceToKeepRange;

        public override void VariableSetting()
        {
            base.VariableSetting();
            SetBlackboardVariable<float>(BTVariables.MoveDistance, moveDistance);
            SetBlackboardVariable<float>(BTVariables.DistanceToKeepRange, distanceToKeepRange);
        }
    }
}