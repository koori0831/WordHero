using Code.FSM;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Player.Code.States
{
	public class PlayerCanAttackStates : PlayerStates
	{
		public PlayerCanAttackStates(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
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
