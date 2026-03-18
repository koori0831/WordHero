using Code.FSM;
using Work.Agents.Code;
using Work.Input.Code;
using Work.Players.Code.States;
using Work.Weapons.Code;

namespace Work.Players.Code
{
    public class Player : Agent
    {
        private StateMachine _stateMachine;
        private PlayerInputModule _inputRoot;
        private PlayerMovementModule _movementModule;
        private PlayerWeaponModule _weaponModule;

        private PlayerHealthModule _health;

        private void Awake()
        {
            Init();
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
        private void OnHit() => _stateMachine.ChangeState(PlayerStateKeys.Hit);

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
    }
}
