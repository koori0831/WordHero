using Code.FSM;
using Code.Entities;
using Work.Core.Utils.EventBus;

namespace Work.Player.Code.States
{
	public class PlayerCanAttackStates : PlayerStates
	{
		public PlayerCanAttackStates(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
		{
			Bus<PlayerRequestAttackEvent>.Events += OnRequestAttack;
		}

        ~PlayerCanAttackStates()
		{
			Bus<PlayerRequestAttackEvent>.Events -= OnRequestAttack;
        }

        private void OnRequestAttack(PlayerRequestAttackEvent @event)
        {
			_stateMachine.ChangeState("Attack");
        }
    }
}