using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPatralPoints", story: "[Self] set patrol [points] [number] [range]", category: "Action", id: "563799d3c86759b452078352415edc27")]
public partial class SetPatralPointsAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Points;
    [SerializeReference] public BlackboardVariable<int> Number;
    [SerializeReference] public BlackboardVariable<float> range;

    protected override Status OnStart()
    {
        Points.Value.Clear();
        EnemyMovementModule mover = Self.Value.GetModule<EnemyMovementModule>();

        Vector3 pos = Self.Value.transform.position;

        for(int i = 0; i < Number.Value; i++)
        {
            Vector3 patrolPoint = pos + UnityEngine.Random.insideUnitSphere * range;
            patrolPoint.y = pos.y;
            int count = 0;
            while (!mover.CanMovePoint(patrolPoint) && count <= 20)
            {

                patrolPoint = pos + UnityEngine.Random.insideUnitSphere * range;
                patrolPoint.y = pos.y;
                count++;
            }

            
            Points.Value.Add(patrolPoint);
        }

        return Status.Success;
    }
}

