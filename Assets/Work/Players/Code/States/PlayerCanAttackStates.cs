using Code.FSM;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Weapons.Code;

namespace Work.Players.Code.States
{
	public class PlayerCanAttackStates : PlayerStates
	{
        private PlayerWeaponModule _weaponModule;

		public PlayerCanAttackStates(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
		{
			Bus<InputAttackEvent>.Events += OnRequestAttack;
			Bus<InputDodgeEvent>.Events += OnRequestDodge;

            _weaponModule = owner.GetModule<PlayerWeaponModule>(true);
        }

        public override void Dispose()
		{
            Bus<InputAttackEvent>.Events -= OnRequestAttack;
            Bus<InputDodgeEvent>.Events -= OnRequestDodge;
        }

        protected virtual void OnRequestAttack(InputAttackEvent @event)
        {
            if (_weaponModule.CurrentWeapon == null) return;

            switch (_weaponModule.WeaponType)
            {
                case WeaponType.Melee:
                    _stateMachine.ChangeState(PlayerStateKeys.MeleeAttack);
                    break;
                case WeaponType.Ranged:
                    _stateMachine.ChangeState(PlayerStateKeys.RangedAttack);
                    break;

                default:
                    Debug.LogError($"Unsupported weapon type: {_weaponModule.WeaponType}");
                    break;
            }
        }

        private void OnRequestDodge(InputDodgeEvent @event)
        {
            _stateMachine.ChangeState(PlayerStateKeys.Dodge);
        }
    }
}
