using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AnimationEndCheck", story: "[Self] animtion end check", category: "Action", id: "6f50d3be169a1db4e437e55847217210")]
public partial class AnimationEndCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    private EnemyAnimationTriggerModule _animationTrigger;
    private bool _animationEnded = false;

    protected override Status OnStart()
    {
        _animationTrigger = Self.Value.GetModule<EnemyAnimationTriggerModule>();
        Debug.Assert(_animationTrigger != null, $"{nameof(EnemyAnimationTriggerModule)} is null");
        _animationTrigger.OnAnimationEnd += HandleAnimationEndTrigger;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        if(_animationEnded)
        {
            _animationEnded = false;
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        _animationTrigger.OnAnimationEnd -= HandleAnimationEndTrigger;
    }

    private void HandleAnimationEndTrigger()
    {
        _animationEnded = true; 
    }
}

