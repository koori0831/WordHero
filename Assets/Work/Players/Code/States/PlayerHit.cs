using Code.FSM;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerHit : PlayerStates
    {
        public PlayerHit(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
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
