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

        public PlayerMeleeAttack(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
            _weaponModule = owner.GetModule<PlayerWeaponModule>(true);
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
                    _stateMachine.ChangeState(PlayerStateKeys.MeleeAttack, true);
                }
                else if(_queueNext)
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState(PlayerStateKeys.MeleeAttack, true);
                }
                else
                {
                    _comboIndex = 0;
                    _stateMachine.ChangeState(PlayerStateKeys.Idle);
                }

                _weaponModule.CurrentWeapon?.GetComponent<MeleeWeapon>()?.EndAttack();
            }
        }

        private void AttackProcess()
        {
            if (_weaponModule.WeaponType == WeaponType.Melee)
            {
                _weaponModule.CurrentWeapon?.GetComponent<MeleeWeapon>()?.StartAttack();
            }
            else 
            {
                Debug.LogWarning("PlayerMeleeAttack state entered but weapon is not melee.");
            }
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
            _weaponModule.CurrentWeapon?.GetComponent<MeleeWeapon>()?.EndAttack();
            base.Exit();
        }
    }
}
