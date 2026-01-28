using UnityEngine;
using System.Collections;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerDodge : PlayerStates
    {
        EntityMover _mover;
        public PlayerDodge(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            _mover = _entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();

            //_mover.AddImpulse
        }
    }
}