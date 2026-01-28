using Code.FSM;
using Code.Entities;
using Work.Core.Utils.EventBus;
using System;

namespace Work.Player.Code.States
{
	public class PlayerCanAttackStates : PlayerStates
	{
		public PlayerCanAttackStates(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
		{
			Bus<PlayerRequestAttackEvent>.Events += OnRequestAttack;
			Bus<PlayerRequestDodgeEvent>.Events += OnRequestDodge;
        }

        ~PlayerCanAttackStates()
		{
			Bus<PlayerRequestAttackEvent>.Events -= OnRequestAttack;
            Bus<PlayerRequestDodgeEvent>.Events -= OnRequestDodge;
        }

        private void OnRequestAttack(PlayerRequestAttackEvent @event)
        {
            _stateMachine.ChangeState("Attack");
        }

        private void OnRequestDodge(PlayerRequestDodgeEvent @event)
        {
            _stateMachine.ChangeState("Dodge");
        }
    }
}