using UnityEngine;
using Code.FSM;
using Work.Agents.Code;

namespace Work.Player.Code.States
{
    public class PlayerMove : PlayerCanAttackStates
    {
        private PlayerInputModule _input;
        private PlayerMovementModule _mover;
        private static int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static int _moveInputHash = Animator.StringToHash("MoveInput");
        private const float _idleTransitionBufferTime = 0.06f;
        private float _idleTransitionTimer;


        public PlayerMove(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
            if (_player == null) return;
            _input = _player.GetModule<PlayerInputModule>(true);
            _mover = _player.GetModule<PlayerMovementModule>(true);
        }

        public override void Update()
        {
            base.Update();
            Vector2 moveVector = _input.MoveVector;

            bool hasMoveInput = moveVector.sqrMagnitude > 0.01f || _input.IsMovePressed;

            if (hasMoveInput)
            {
                _idleTransitionTimer = _idleTransitionBufferTime;
                _mover.Move(moveVector);
                _animator.SetParam(_moveSpeedHash, _mover.Speed);
                _animator.SetParam(_moveInputHash, moveVector.magnitude);
            }
            else
            {
                _idleTransitionTimer -= Time.deltaTime;
                if (_idleTransitionTimer <= 0f)
                {
                    _stateMachine.ChangeState("Idle");
                }
            }
        }

        public override void Enter()
        {
            base.Enter();
            _idleTransitionTimer = _idleTransitionBufferTime;
            _animator.SetApplyRootMotion(true);
        }

        public override void Exit()
        {
            _animator.SetApplyRootMotion(false);
            base.Exit();
        }
    }
}
