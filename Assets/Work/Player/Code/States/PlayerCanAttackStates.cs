using Code.FSM;
using Code.Entities;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Player.Code.States
{
	public class PlayerCanAttackStates : PlayerStates
	{
		public PlayerCanAttackStates(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
		{
			Bus<InputAttackEvent>.Events += OnRequestAttack;
			Bus<InputDodgeEvent>.Events += OnRequestDodge;
        }

        public override void Dispose()
		{
            Bus<InputAttackEvent>.Events -= OnRequestAttack;
            Bus<InputDodgeEvent>.Events -= OnRequestDodge;
        }

        protected virtual void OnRequestAttack(InputAttackEvent @event)
        {
            _stateMachine.ChangeState("Attack");
        }

        private void OnRequestDodge(InputDodgeEvent @event)
        {
            _stateMachine.ChangeState("Dodge");
        }
    }
}
