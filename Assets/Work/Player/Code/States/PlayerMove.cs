using UnityEngine;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerMove : PlayerCanAttackStates
    {
        private PlayerInputRoot _input;
        private EntityMover _mover;
        private static int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static int _moveInputHash = Animator.StringToHash("MoveInput");
        private const float _idleTransitionBufferTime = 0.06f;
        private float _idleTransitionTimer;


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
