using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Work.Enemies.Code;

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
        animModule.SetParam(currentAnimHash, false);
        animModule.SetParam(nextAnimHash, true);
        CurrentAnimation.Value = NextAnimation.Value;
        return Status.Success;
    }
}

