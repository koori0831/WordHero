using Code.FSM;
using System;
using Unity.Behavior;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;
using Work.Enemies.Code;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "StatusCheck", story: "[self] check have [status]", category: "Conditions", id: "11cb0db1c47f80862f6faa4f68dc96ae")]
public partial class StatusCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<StatusType> Status;

    public override bool IsTrue() => Self.Value.GetModule<AgentStatusModule>(true).HasStatusEffect(Status.Value);
}
