using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Interaction.Code;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;

namespace Work.Players.Code
{
    public class PlayerWeaponModule : MonoBehaviour, IAgentModule
    {
        private const float DefaultImprintTriggerDuration = 3f;

        private Player _owner;
        private PlayerWeaponInventory _inventory;
        private PlayerWeaponController _controller;
        private PlayerAnimationModule _animationModule;
        private ImprintActivationRunner _imprintActivationRunner;

        private WeaponRuntimeInstance _currentRuntime;
        private WeaponRuntimeInstance _standbyRuntime;

        private Action _unsubscribeStandbyTrigger;

        public BaseWeapon CurrentWeapon => _inventory?.CurrentWeapon;
        public BaseWeapon StandbyWeapon => _inventory?.StandbyWeapon;

        [field: SerializeField] public Transform WeaponHandler { get; private set; }

        #region Weapon Properties
        public bool IsCanSwapWeapon => _inventory?.CanSwap ?? false;
        public bool HaveWeapon => _inventory?.HaveWeapon ?? false;
        public WeaponType WeaponType => CurrentWeapon?.Data.Type ?? WeaponType.OneHandSword;
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
            _imprintActivationRunner = new ImprintActivationRunner();

            Bus<WeaponSwapEvent>.Events -= OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events -= OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events -= OnSecondarySkill;
            Bus<SkillMotionEndEvent>.Events -= OnSkillMotionEnd;

            Bus<WeaponSwapEvent>.Events += OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events += OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events += OnSecondarySkill;
            Bus<SkillMotionEndEvent>.Events += OnSkillMotionEnd;
        }

        private void Update()
        {
            _standbyRuntime?.TriggerRuntime?.Update();
        }

        private void OnDestroy()
        {
            Bus<WeaponSwapEvent>.Events -= OnWeaponSwap;
            Bus<FirstWeaponSkillEvent>.Events -= OnPrimarySkill;
            Bus<SecondWeaponSkillEvent>.Events -= OnSecondarySkill;
            Bus<SkillMotionEndEvent>.Events -= OnSkillMotionEnd;

            UnsubscribeStandbyTriggerEvent();
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
            TryActivateCurrentWeaponImprint();
        }

        private void OnChangedWeapon()
        {
            _inventory.SetOwner(_owner);
            _controller.ApplyVisualState(CurrentWeapon, StandbyWeapon);
            _animationModule.SetAnimationController(CurrentWeapon?.Data.AnimSet);

            SyncRuntimeInstances();
            RefreshStandbyTriggerSubscription();
        }

        private void SyncRuntimeInstances()
        {
            WeaponRuntimeInstance prevCurrent = _currentRuntime;
            WeaponRuntimeInstance prevStandby = _standbyRuntime;

            _currentRuntime = FindOrCreateRuntime(CurrentWeapon, prevCurrent, prevStandby);
            _standbyRuntime = FindOrCreateRuntime(StandbyWeapon, prevCurrent, prevStandby);
        }

        private WeaponRuntimeInstance FindOrCreateRuntime(BaseWeapon targetWeapon, WeaponRuntimeInstance prevCurrent, WeaponRuntimeInstance prevStandby)
        {
            if (targetWeapon == null)
                return null;

            if (prevCurrent != null && prevCurrent.Weapon == targetWeapon)
                return prevCurrent;

            if (prevStandby != null && prevStandby.Weapon == targetWeapon)
                return prevStandby;

            return new WeaponRuntimeInstance(targetWeapon);
        }

        private void RefreshStandbyTriggerSubscription()
        {
            UnsubscribeStandbyTriggerEvent();

            IImprintTriggerEvent triggerEvent = _standbyRuntime?.Weapon?.Data?.ImprintTriggerEvent;
            if (triggerEvent == null)
                return;

            _unsubscribeStandbyTrigger = triggerEvent.Subscribe(OpenStandbyTrigger);
        }

        private void UnsubscribeStandbyTriggerEvent()
        {
            _unsubscribeStandbyTrigger?.Invoke();
            _unsubscribeStandbyTrigger = null;
        }

        private void OpenStandbyTrigger()
        {
            if (_standbyRuntime == null)
                return;

            _standbyRuntime.TriggerRuntime.Open(DefaultImprintTriggerDuration);

            if (!_standbyRuntime.Weapon.Imprints.HasAnyImprint())
                return;

            Bus<WeaponTriggerOpenedEvent>.Raise(new WeaponTriggerOpenedEvent(DefaultImprintTriggerDuration));
        }

        private void TryActivateCurrentWeaponImprint()
        {
            if (_currentRuntime?.Weapon == null)
                return;

            if (!_currentRuntime.TriggerRuntime.CanActivate())
                return;

            if (!_currentRuntime.Weapon.Imprints.HasAnyImprint())
            {
                _currentRuntime.TriggerRuntime.Consume();
                return;
            }

            SkillContext context = new SkillContext(
                _owner,
                _owner.transform.position,
                _owner.transform.forward
            );

            _imprintActivationRunner.Activate(_currentRuntime, context);
            Bus<WeaponTriggerActivatedEvent>.Raise(new WeaponTriggerActivatedEvent());
            _currentRuntime.TriggerRuntime.Consume();
        }

        private void OnWeaponSwap(WeaponSwapEvent evt)
        {
            if (IsCanSwapWeapon)
                SwapWeapon();
        }

        private void OnPrimarySkill(FirstWeaponSkillEvent evt)
        {
            if (evt.isReleased) return;

            if (_owner != null && _owner.gameObject.activeInHierarchy)
                CurrentWeapon?.UsePrimary(null, _owner.transform.forward);
        }

        private void OnSecondarySkill(SecondWeaponSkillEvent evt)
        {
            if (evt.isReleased) return;

            if (_owner != null && _owner.gameObject.activeInHierarchy)
                CurrentWeapon?.UseSecondary(null, _owner.transform.forward);
        }

        private void OnSkillMotionEnd(SkillMotionEndEvent evt)
        {
            if (CurrentWeapon == null) return;
            CurrentWeapon.IsSkillUsing = false;
        }
    }
}
