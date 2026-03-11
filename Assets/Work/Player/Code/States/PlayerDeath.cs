using Code.FSM;
using Work.Agents.Code;

namespace Work.Player.Code.States
{
    public class PlayerDeath : PlayerStates
    {
        public PlayerDeath(StateMachine stateMachine, Agent owner, int animationHash) : base(stateMachine, owner, animationHash)
        {
        }
    }
}
