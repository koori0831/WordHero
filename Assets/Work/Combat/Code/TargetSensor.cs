using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using Work.Entities;

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



        public List<IDamageable> Cast()
        {
            List<IDamageable> damageables = new List<IDamageable>();
            Collider[] cols = new Collider[0];

            cols = targetingShape switch
            {
                TargetingShape.Box => Physics.OverlapBox(transform.position, boxSize, transform.rotation, targetLayer),
                TargetingShape.Circle => Physics.OverlapSphere(transform.position, circleRadius, targetLayer),
                _=> new Collider[0],
            };

            foreach (Collider col in cols)
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageables.Add(damageable);
                }
            }

            return damageables;
        }

    }
}