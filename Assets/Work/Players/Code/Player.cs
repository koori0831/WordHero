using Code.FSM;
using GondrLib.Dependencies;
using NUnit.Framework.Constraints;
using UnityEngine;
using Work.AcquireItem.Code;
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

        private PlayerImprintWordInventory _imprintWordInventory;

        public PlayerImprintWordInventory ImprintWordInventory => _imprintWordInventory;
        public PlayerWeaponModule WeaponModule => _weaponModule;

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

            _imprintWordInventory = new PlayerImprintWordInventory();
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
            Bus<OnGetItemEvent>.Raise(new OnGetItemEvent(weapon.Data.WeaponName, "무기", Color.red));
        }

        public void GetImprintWord(ImprintWordSO imprintWord, int amount)
        {
            _imprintWordInventory.AddImprintWord(imprintWord, amount);
            string type = "각인";
            Color color = Color.white;
            switch (imprintWord.Type)
            {
                case ImprintType.Attack:
                    type = "공격";
                    color = Color.blue;
                    break;
                case ImprintType.Stat:
                    type = "능력";
                    color = Color.purple;
                    break;
                case ImprintType.Effect:
                    type = "효과";
                    color = ColorUtility.TryParseHtmlString("#009A19", out Color effectColor) ? effectColor : Color.white;
                    break;
            }
            Bus<OnGetItemEvent>.Raise(new OnGetItemEvent(imprintWord.DisplayName, type, color));
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
