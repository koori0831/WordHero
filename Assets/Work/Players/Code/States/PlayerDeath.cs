using Code.FSM;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerDeath : PlayerStates
    {
        public PlayerDeath(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
        {
        }
    }
}
