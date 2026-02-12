using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NavAgentObstacleSetting", story: "[Self] set navAgent Obstacle setting", category: "Action", id: "d381fddba7a2c1abdc3f793b9ba9a8f3")]
public partial class NavAgentObstacleSettingAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    protected override Status OnStart()
    {
        Self.Value.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        return Status.Running;
    }
}

