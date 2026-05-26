using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetAttackPhase", story: "[Self] set attack phase [Phase] to [ParameterName]", category: "Action", id: "ac6e97a209e44b258a86f93e341f0f21")]
public partial class SetAttackPhaseAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<int> Phase;
    [SerializeReference] public BlackboardVariable<string> ParameterName;

    protected override Status OnStart()
    {
        EnemyAnimatorModule animator = Self.Value.GetModule<EnemyAnimatorModule>();
        Debug.Assert(animator != null, "EnemyAnimatorModule is missing.");

        string parameterName = string.IsNullOrEmpty(ParameterName.Value) ? "ATTACK_PHASE" : ParameterName.Value;
        animator.SetParam(Animator.StringToHash(parameterName), Phase.Value);
        return Status.Success;
    }
}
