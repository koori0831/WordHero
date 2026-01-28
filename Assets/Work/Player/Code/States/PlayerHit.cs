using UnityEngine;
using System.Collections;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerHit : PlayerStates
    {
        public PlayerHit(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);
            if (eventType == AnimationEventType.End)
            {
                _stateMachine.ChangeState("Idle");
            }
        }
    }
}