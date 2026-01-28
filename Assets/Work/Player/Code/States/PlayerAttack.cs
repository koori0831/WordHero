using UnityEngine;
using System.Collections;
using Code.FSM;
using Code.Entities;
using System;

namespace Work.Player.Code.States
{
    public class PlayerAttack : PlayerStates
    {
        public PlayerAttack(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);
            if (eventType == AnimationEventType.Attack)
            {
                AttackProcess();
            }
            if (eventType == AnimationEventType.End)
            {
                _stateMachine.ChangeState("Idle");
            }
        }

        private void AttackProcess()
        {
        }
    }
}