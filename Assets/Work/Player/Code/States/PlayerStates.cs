using UnityEngine;
using System.Collections;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerStates : State
    {
        protected Player _player;

        public PlayerStates(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            _player = entity as Player;
            Debug.Assert(_player != null, "PlayerStates can only be used with Player entity.");
        }
    }
}