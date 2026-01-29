using System;
using Unity.Behavior;
using UnityEngine;
using Work.Enemies.Code;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsInDetect", story: "[Self] check [detectRange] from [Target]", category: "Conditions", id: "da21a40836326ec2817d68cf56e165f9")]
public partial class IsInDetectCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<float> DetectRange;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
