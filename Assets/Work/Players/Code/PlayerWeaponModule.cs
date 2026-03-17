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

        public BaseWeapon CurrentWeapon => _inventory != null ? _inventory.CurrentWeapon : null;
        public BaseWeapon StandbyWeapon => _inventory != null ? _inventory.StandbyWeapon : null;

        [field: SerializeField] public Transform WeaponHandler { get; private set; }

        #region Weapon Properties
        public bool IsCanSwapWeapon => _inventory != null && _inventory.CanSwap;

        public float AttackRange => CurrentWeapon != null ? CurrentWeapon.Data.Range : 0f;

        public float AttackSpeed => CurrentWeapon != null ? CurrentWeapon.Data.AttackSpeed : 0f;

        public float AttackDamage => CurrentWeapon != null ? CurrentWeapon.Data.BaseDamage : 0f;

        public WeaponType WeaponType => CurrentWeapon != null ? CurrentWeapon.Data.Type : WeaponType.Melee; 
        #endregion

        public void Initialize(Agent agent)
        {
            _owner = agent as Player;
            Debug.Assert(_owner != null, "PlayerWeaponModule can only be initialized with a Player agent.");

            WeaponHandler.localPosition = Vector3.zero;
            WeaponHandler.localRotation = Quaternion.identity;

            _inventory = new PlayerWeaponInventory();
            _controller = new PlayerWeaponController(WeaponHandler);

            Bus<WeaponSwapEvent>.Events += OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events += OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events += OnSecondarySkill;
        }

        private void OnDestroy()
        {
            Bus<WeaponSwapEvent>.Events -= OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events -= OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events -= OnSecondarySkill;
        }

        public void EquipWeapon(BaseWeapon weapon)
        {
            if (weapon == null || _owner == null) return;

            BaseWeapon droppedWeapon = _inventory.Equip(weapon);
            if (droppedWeapon != null)
            {
                Vector3 dropPosition = _owner.transform.position + (_owner.transform.forward * 1.2f);
                DropService.DropWeapon(droppedWeapon, dropPosition);
            }

            _inventory.SetOwner(_owner);
            _controller.ApplyVisualState(CurrentWeapon, StandbyWeapon);
        }

        public void SwapWeapon()
        {
            if (!_inventory.Swap()) return;

            _inventory.SetOwner(_owner);
            _controller.ApplyVisualState(CurrentWeapon, StandbyWeapon);

            // TODO: 대기 상태로 전환시, 트리거 체크를 시작하도록 해야함.
        }

        public void HandlerSetting(WeaponDataSO weaponData)
        {
            if (CurrentWeapon == null || weaponData == null) return;
            _controller.ApplyVisualState(CurrentWeapon, StandbyWeapon);
        }

        public void CastPrimarySkill(Transform target, Vector3 direction)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.UsePrimary(target, direction);
            }
        }

        public void CastSecondarySkill(Transform target, Vector3 direction)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.UseSecondary(target, direction);
            }
        }

        public void CastTriggerSkill(Transform target, Vector3 direction)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.UseTrigger(target, direction);
            }
        }

        private void OnWeaponSwap(WeaponSwapEvent evt)
        {
            if (IsCanSwapWeapon)
            {
                SwapWeapon();
            }
        }

        private void OnPrimarySkill(FirstWeaponSkillEvent evt)
        {
            if (_owner == null) return;
            CastPrimarySkill(null, _owner.transform.forward);
        }

        private void OnSecondarySkill(SecondWeaponSkillEvent evt)
        {
            if (_owner == null) return;
            CastSecondarySkill(null, _owner.transform.forward);
        }
    }
}
