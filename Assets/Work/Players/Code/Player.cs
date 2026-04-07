using UnityEngine;
using Code.FSM;
using GondrLib.Dependencies;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Players.Code.States;
using Work.Stages.Code;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;

namespace Work.Players.Code
{
    [Provide]
    public class Player : Agent, IDependencyProvider
    {
        private const string DEFAULTLAYER = "Player";
        private const string VANISHLAYER = "PlayerDodge";

        private StateMachine _stateMachine;
        private PlayerInputModule _inputRoot;
        private PlayerMovementModule _movementModule;
        private PlayerWeaponModule _weaponModule;

        private PlayerHealthModule _health;

        public bool HaveWeapon => _weaponModule.HaveWeapon;

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            if (StageManager.CurrentStage != null)
                transform.position = StageManager.CurrentStage.SpawnPoint;
        }

        public override void Init()
        {
            base.Init();
            _health = GetModule<PlayerHealthModule>(true);
            _inputRoot = GetModule<PlayerInputModule>(true);
            _movementModule = GetModule<PlayerMovementModule>(true);
            _weaponModule = GetModule<PlayerWeaponModule>(true);
            AgentStateModule stateModule = GetModule<AgentStateModule>(true);
            _stateMachine = stateModule.StateMachine;

            OnHitEvent.AddListener(_health.TakeDamage);
            _health.Damaged += OnHit;
            _health.OnDeath.AddListener(OnDead);
        }

        private void OnDead() => _stateMachine.ChangeState(PlayerStateKeys.Death);
        private void OnHit()
        {
            Bus<OnTookDamageTrigger>.Raise(new OnTookDamageTrigger());
            _stateMachine.ChangeState(PlayerStateKeys.Hit);
        }

        public void LockOn()
        {
            if (_inputRoot.Input.CurrentDeviceType == InputDeviceType.KeyboardMouse)
                _movementModule.RotateToMousePosition();
        }

        public void GetWeapon(BaseWeapon weapon)
        {
            _weaponModule.EquipWeapon(weapon);
        }

        public void ChangeState(string stateKey)
        {
            _stateMachine.ChangeState(stateKey);
        }

        public void SetVanished(bool vanished)
        {
            gameObject.layer = LayerMask.NameToLayer(vanished ? VANISHLAYER : DEFAULTLAYER);
        }
    }
}
