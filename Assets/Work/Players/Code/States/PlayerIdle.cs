using Code.FSM;
using UnityEngine;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerIdle : PlayerCanAttackStates
    {
        private PlayerInputModule _input;

        public PlayerIdle(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
            if (_player == null) return;
            _input = _player.GetModule<PlayerInputModule>(true);
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveVector = _input.MoveVector;
            if (moveVector.sqrMagnitude > 0.01f || _input.IsMovePressed)
            {
                _stateMachine.ChangeState(PlayerStateKeys.Move);
            }
        }
    }
}
