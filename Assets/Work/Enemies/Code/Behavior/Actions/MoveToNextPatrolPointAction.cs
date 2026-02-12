using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToNextPatrolPoint", story: "[Self] move to next patrol [point] in [Points]", category: "Action", id: "9b20d34e9cdb7f654faa2c08531e36c7")]
public partial class MoveToNextPatrolPointAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Vector3> Point;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Points;

    protected override Status OnStart()
    {
        Enemy enemy = Self.Value;
        List<Vector3> patrolPoints = Points.Value;

        Vector3 nextPoint = Point.Value;

        while (nextPoint == Point.Value)
        {
            nextPoint = patrolPoints[UnityEngine.Random.Range(0, patrolPoints.Count)];
        }

        Point.Value = nextPoint;

        enemy.GetModule<EnemyMovementModule>().SetDestination(nextPoint);

        return Status.Success;
    }
}

