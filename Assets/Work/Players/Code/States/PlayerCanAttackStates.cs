using Code.FSM;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Players.Code.States
{
    public class PlayerCanAttackStates : PlayerStates
    {
        public PlayerCanAttackStates(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
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
            if (_stateMachine.CurrentState != this) return;
            if (_player.HaveWeapon == false) return;
            _stateMachine.ChangeState(PlayerStateKeys.Attack);
        }

        protected virtual void OnRequestDodge(InputDodgeEvent @event)
        {
            if (_stateMachine.CurrentState != this) return;
            _stateMachine.ChangeState(PlayerStateKeys.Dodge);
        }
    }
}
