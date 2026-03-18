using Code.FSM;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerSlash : PlayerStates
    {
        public PlayerSlash(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _animator.SetApplyRootMotion(true);
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);

            if (eventType == AnimationEventType.End)
            {
                _stateMachine.ChangeState(PlayerStateKeys.Idle);
            }
        }

        public override void Exit()
        {
            _animator.SetApplyRootMotion(false);
            base.Exit();
        }
    }
}
