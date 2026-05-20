using Code.FSM;
using UnityEngine;
using Work.Input.Code;
using Work.Agents.Code;
using Work.Weapons.Code;

namespace Work.Players.Code.States
{
    public class PlayerMeleeAttack : PlayerCanAttackStates
    {
        private const int MaxCombo = 3;
        private const float ComboResetGrace = 0.15f;
        private static readonly int _attackIndexHash = Animator.StringToHash("AttackIndex");

        private int _comboIndex;
        private bool _queueNext;
        private bool _comboWindowOpen;
        private float _comboExpireTime;

        private PlayerWeaponModule _weaponModule;

        public PlayerMeleeAttack(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
        {
            _weaponModule = owner.GetModule<PlayerWeaponModule>(true);
        }

        protected override void OnRequestAttack(InputAttackEvent @event)
        {
            if (_stateMachine.CurrentState != this) return;
            if (_comboWindowOpen)
                _queueNext = true;
        }

        protected override void OnRequestDodge(InputDodgeEvent @event)
        {
            return;
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
                    _stateMachine.ChangeState(PlayerStateKeys.Attack, true);
                }
                else if(_queueNext)
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState(PlayerStateKeys.Attack, true);
                }
                else
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState(PlayerStateKeys.Idle);
                }
            }
        }

        private void AttackProcess()
        {
            _weaponModule.CurrentWeapon?.Attack(_comboIndex);
        }

        public override void Enter()
        {
            base.Enter();
            if (!(_stateMachine.PreviousState is PlayerMeleeAttack))
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
