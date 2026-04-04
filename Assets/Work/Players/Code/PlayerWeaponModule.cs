using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Interaction.Code;
using Work.Weapons.Code;

namespace Work.Players.Code
{
    public class PlayerWeaponModule : MonoBehaviour, IAgentModule
    {
        private Player _owner;
        private PlayerWeaponInventory _inventory;
        private PlayerWeaponController _controller;
        private PlayerAnimationModule _animationModule;

        public BaseWeapon CurrentWeapon => _inventory?.CurrentWeapon;
        public BaseWeapon StandbyWeapon => _inventory?.StandbyWeapon;

        [field: SerializeField] public Transform WeaponHandler { get; private set; }

        #region Weapon Properties
        public bool IsCanSwapWeapon => _inventory?.CanSwap ?? false;

        public bool HaveWeapon => _inventory?.HaveWeapon ?? false;

        public WeaponType WeaponType => CurrentWeapon?.Data.Type ?? WeaponType.Melee; 
        #endregion

        public void Initialize(Agent agent)
        {
            _owner = agent as Player;
            Debug.Assert(_owner != null, "PlayerWeaponModule can only be initialized with a Player agent.");
            _animationModule = _owner.GetModule<PlayerAnimationModule>(true);

            WeaponHandler.localPosition = Vector3.zero;
            WeaponHandler.localRotation = Quaternion.identity;

            _inventory = new PlayerWeaponInventory();
            _controller = new PlayerWeaponController(WeaponHandler);

            Bus<WeaponSwapEvent>.Events += OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events += OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events += OnSecondarySkill;
            Bus<SkillMotionEndEvent>.Events += OnSkillMotionEnd;
        }

        private void OnDestroy()
        {
            Bus<WeaponSwapEvent>.Events -= OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events -= OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events -= OnSecondarySkill;
            Bus<SkillMotionEndEvent>.Events -= OnSkillMotionEnd;
        }

        public void EquipWeapon(BaseWeapon weapon)
        {
            if (weapon == null || _owner == null) return;

            BaseWeapon droppedWeapon = _inventory.Equip(weapon);
            if (droppedWeapon != null)
            {
                droppedWeapon.IsSkillUsing = false;
                Vector3 dropPosition = _owner.transform.position + (_owner.transform.forward * 1.2f);
                DropService.DropWeapon(droppedWeapon, dropPosition);
            }

            OnChangedWeapon();
        }

        public void SwapWeapon()
        {
            if (!_inventory.Swap()) return;

            OnChangedWeapon();

            // TODO: 대기 상태로 전환시, 트리거 체크를 시작하도록 해야함.
        }

        private void OnChangedWeapon()
        {
            _inventory.SetOwner(_owner);
            _controller.ApplyVisualState(CurrentWeapon, StandbyWeapon);
            _animationModule.SetAnimationController(CurrentWeapon?.Data.AnimSet);
        }

        private void OnWeaponSwap(WeaponSwapEvent evt)
        {
            if (IsCanSwapWeapon) SwapWeapon();
        }

        private void OnPrimarySkill(FirstWeaponSkillEvent evt)
        {
            if (_owner != null) CurrentWeapon?.UsePrimary(null, _owner.transform.forward);
        }

        private void OnSecondarySkill(SecondWeaponSkillEvent evt)
        {
            if (_owner != null) CurrentWeapon?.UseSecondary(null, _owner.transform.forward);
        }

        private void OnSkillMotionEnd(SkillMotionEndEvent @event)
        {
            if (CurrentWeapon == null) return;
            CurrentWeapon.IsSkillUsing = false;
        }
    }
}
