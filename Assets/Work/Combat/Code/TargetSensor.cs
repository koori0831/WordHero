using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Enemies.Code;

namespace Work.Combat.Code
{
    public enum TargetingType
    {
        SingleEnemy,
        AreaEnemy,
    }

    public enum TargetingShape
    {
        Circle,
        Box,
    }

    public class TargetSensor : MonoBehaviour
    {
        private Agent _owner;

        [SerializeField] private TargetingType targetingType = TargetingType.AreaEnemy;
        [SerializeField] private TargetingShape targetingShape = TargetingShape.Circle;

        private bool IsSingleTarget => targetingType == TargetingType.SingleEnemy;
        private bool IsAreaTarget => targetingType == TargetingType.AreaEnemy;
        private bool IsBoxShape => targetingShape == TargetingShape.Box;
        private bool IsCircleShape => targetingShape == TargetingShape.Circle;

        //여러마리
        [SerializeField] private LayerMask targetLayer;

        //박스형
        [ShowIf(nameof(IsBoxShape))]
        [SerializeField] private Vector3 boxSize = Vector3.one;

        //원형
        [ShowIf(nameof(IsCircleShape))]
        [SerializeField] private float circleRadius = 0.5f;

        public void Init(Agent enemy)
        {
            _owner = enemy;
        }

        public List<T> Cast<T>() where T : ICastable
        {
            List<T> damageables = new List<T>();
            Collider[] cols = new Collider[0];

            cols = targetingShape switch
            {
                TargetingShape.Box => Physics.OverlapBox(transform.position, boxSize, _owner.transform.rotation, targetLayer),
                TargetingShape.Circle => Physics.OverlapSphere(transform.position, circleRadius, targetLayer),
                _=> new Collider[0],
            };

            foreach (Collider col in cols)
            {
                T damageable = col.GetComponent<T>();
                if (damageable != null)
                {
                    damageables.Add(damageable);
                }
            }

            return damageables;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            switch (targetingShape)
            {
                case TargetingShape.Box:
                    Gizmos.DrawWireCube(transform.position, boxSize * 2);
                    break;
                case TargetingShape.Circle:
                    Gizmos.DrawWireSphere(transform.position, circleRadius);
                    break;
            }
        }

    }
}
