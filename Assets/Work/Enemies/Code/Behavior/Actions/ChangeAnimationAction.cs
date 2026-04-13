using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeAnimationAction", story: "[self] Changed from [currentAnimation] to [nextAnimation]", category: "Action", id: "daac5b4b44587046cbc97b9c1ddf538e")]
public partial class ChangeAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<string> CurrentAnimation;
    [SerializeReference] public BlackboardVariable<string> NextAnimation;
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    protected override Status OnStart()
    {
        int currentAnimHash = Animator.StringToHash(CurrentAnimation.Value);
        int nextAnimHash = Animator.StringToHash(NextAnimation.Value);
        EnemyAnimatorModule animModule = Self.Value.GetModule<EnemyAnimatorModule>();
        if (currentAnimHash != 0)
            animModule.SetParam(currentAnimHash, false);
        animModule.SetParam(nextAnimHash, true);
        CurrentAnimation.Value = NextAnimation.Value;
        return Status.Success;
    }
}

