using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForAttackTrigger", story: "[Self] wait for attack trigger", category: "Action", id: "1135bad27dbd040c8bcacb04a4d0d634")]
public partial class WaitForAttackTriggerAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    private EnemyAnimationTriggerModule _animationTriggerModule;
    private bool _attackTriggered = false;

    protected override Status OnStart()
    {
        _animationTriggerModule = Self.Value.GetModule<EnemyAnimationTriggerModule>();
        Debug.Assert(_animationTriggerModule != null, "EnemyAnimationTriggerModule is missing.");

        _animationTriggerModule.OnAttackEvent += HandleAttackTrigger;

        return Status.Running;
    }

    private void HandleAttackTrigger()
    {
        throw new NotImplementedException();
    }

    protected override Status OnUpdate()
    {
        if (_attackTriggered)
        {
            _attackTriggered = false;
            return Status.Success;
        }

        return Status.Running;

    }

    protected override void OnEnd()
    {
        _animationTriggerModule.OnAttackEvent -= HandleAttackTrigger;
    }
}

