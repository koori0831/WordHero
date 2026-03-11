using Code.FSM;
using Code.Entities;
using UnityEngine;
using Work.Input.Code;

namespace Work.Player.Code.States
{
    public class PlayerAttack : PlayerCanAttackStates
    {
        private const int MaxCombo = 3;
        private const float ComboResetGrace = 0.15f;
        private static readonly int _attackIndexHash = Animator.StringToHash("AttackIndex");

        private int _comboIndex;
        private bool _queueNext;
        private bool _comboWindowOpen;
        private float _comboExpireTime;

        public PlayerAttack(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
        }

        protected override void OnRequestAttack(InputAttackEvent @event)
        {
            if (_comboWindowOpen)
                _queueNext = true;
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);
            if (eventType == AnimationEventType.Attack)
            {
                AttackProcess();
            }
            if (eventType == AnimationEventType.ComboWindowOpen)
            {
                _comboWindowOpen = true;
            }
            if (eventType == AnimationEventType.ComboWindowClose)
            {
                _comboWindowOpen = false;
            }
            if (eventType == AnimationEventType.End)
            {
                if (_queueNext && _comboIndex < MaxCombo - 1)
                {
                    _comboIndex++;
                    _stateMachine.ChangeState("Attack", true);
                }
                else if(_queueNext)
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState("Attack", true);
                }
                else
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState("Idle");
                }
            }
        }

        private void AttackProcess()
        {
            _player.Attack();
        }

        public override void Enter()
        {
            base.Enter();
            if (!(_stateMachine.PreviousState is PlayerAttack))
            {
                if (Time.time > _comboExpireTime)
                    _comboIndex = 0;
            }

            _player.LockOn();

            _queueNext = false;
            _comboWindowOpen = false;

            _animator.SetParam(_attackIndexHash, _comboIndex);
            float clipLength = _animator.GetStateLength(0);
            if (clipLength <= 0f)
                clipLength = 0.7f;
            _comboExpireTime = Time.time + clipLength + ComboResetGrace;
            _animator.SetApplyRootMotion(true);
        }

        public override void Exit()
        {
            _animator.SetApplyRootMotion(false);
            base.Exit();
        }
    }
}
