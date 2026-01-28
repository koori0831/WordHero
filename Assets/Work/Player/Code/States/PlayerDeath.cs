using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerDeath : PlayerStates
    {
        public PlayerDeath(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
        }
    }
}