using Code.Entities;
using Code.FSM;
using UnityEngine;

namespace Work.Player.Code.States
{
    public class PlayerIdle : PlayerCanAttackStates
    {
        private PlayerInputRoot _input;

        public PlayerIdle(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            if (_player == null) return;
            _input = _player.GetCompo<PlayerInputRoot>();
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveVector = _input.MoveVector;
            if (moveVector.sqrMagnitude > 0.01f || _input.IsMovePressed)
            {
                _stateMachine.ChangeState("Move");
            }
        }
    }
}
