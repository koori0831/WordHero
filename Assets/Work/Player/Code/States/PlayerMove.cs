using UnityEngine;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerMove : PlayerCanAttackStates
    {
        private PlayerInputRoot _input;
        private EntityMover _mover;
        public PlayerMove(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            if (_player == null) return;
            _input = _entity.GetCompo<PlayerInputRoot>();
            _mover = _entity.GetCompo<EntityMover>();
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveVector = _input.MoveVector;

            if (moveVector.magnitude < 0.1f)
            {
                _stateMachine.ChangeState("Idle");
            }
            else
            {
                _mover.Move(moveVector);
            }
        }
    }
}