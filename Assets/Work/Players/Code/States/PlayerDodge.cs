using Code.FSM;
using Work.Core.Utils.EventBus;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Players.Code.States
{
    public class PlayerDodge : PlayerStates
    {
        private AgentStatusModule _statusModule;
        private PlayerInputModule _inputRoot;
        private PlayerMovementModule _mover;

        public PlayerDodge(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
        {
            _statusModule = _player.GetModule<AgentStatusModule>(true);
            _inputRoot = _player.GetModule<PlayerInputModule>(true);
            _mover = _player.GetModule<PlayerMovementModule>(true);
        }

        public override void Enter()
        {
            base.Enter();

            _mover.Move(_inputRoot.MoveVector, false); 

            _statusModule.AddStatus(new StatusEffect { type = StatusType.HitImmunity, isInfinite = true });
            _animator.SetApplyRootMotion(true);
            Bus<CombatDodgeEvent>.Raise(new CombatDodgeEvent(_player.gameObject));
        }

        public override void Exit()
        {
            _statusModule.RemoveStatus(StatusType.HitImmunity);
            _animator.SetApplyRootMotion(false);
            base.Exit();
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);
            if (eventType == AnimationEventType.End)
            {
                _stateMachine.ChangeState(PlayerStateKeys.Idle);
            }
        }
    }
}
