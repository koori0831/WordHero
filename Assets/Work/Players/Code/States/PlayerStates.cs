using UnityEngine;
using Code.FSM;
using Work.Agents.Code;

namespace Work.Players.Code.States
{
    public class PlayerStates : State
    {
        protected Player _player;
        public PlayerStates(StateMachine stateMachine, Agent owner, int animationHash, bool isSkillAnimation) : base(stateMachine, owner, animationHash, isSkillAnimation)
        {
            _player = owner as Player;
            Debug.Assert(_player != null, "PlayerStates can only be used with Player owner.");
        }
    }
}
