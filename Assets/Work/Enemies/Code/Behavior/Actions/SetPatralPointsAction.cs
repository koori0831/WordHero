using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetPatralPoints", story: "[Self] set patral [points] [number]", category: "Action", id: "563799d3c86759b452078352415edc27")]
public partial class SetPatralPointsAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Points;
    [SerializeReference] public BlackboardVariable<int> Number;

    protected override Status OnStart()
    {
        Vector3 pos = Self.Value.transform.position;

        for(int i = 0; i < Number.Value; i++)
        {
            Vector3 patralPoint = pos + UnityEngine.Random.insideUnitSphere * 2.5f;

            patralPoint.y = pos.y;
            Points.Value.Add(patralPoint);
        }

        return Status.Success;
    }
}

