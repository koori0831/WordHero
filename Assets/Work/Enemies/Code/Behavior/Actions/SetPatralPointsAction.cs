using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPatralPoints", story: "[Self] set patral [points] [number] [range]", category: "Action", id: "563799d3c86759b452078352415edc27")]
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
            Vector3 patralPoint = pos + UnityEngine.Random.insideUnitSphere * range;
            patralPoint.y = pos.y;
            int count = 0;
            while (!mover.CanMovePoint(patralPoint) && count <= 20)
            {

                patralPoint = pos + UnityEngine.Random.insideUnitSphere * range;
                patralPoint.y = pos.y;
                Debug.Log($"move path is complate : {mover.CanMovePoint(patralPoint)}");
                count++;
            }

            
            Points.Value.Add(patralPoint);
        }

        return Status.Success;
    }
}

