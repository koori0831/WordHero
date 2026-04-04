using Code.FSM;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerHit : PlayerCanAttackStates
    {
        public PlayerHit(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
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
