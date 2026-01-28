using UnityEngine;
using System.Collections;
using Code.FSM;
using Code.Entities;

namespace Work.Player.Code.States
{
    public class PlayerAttack : PlayerStates
    {
        public PlayerAttack(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
        }


    }
}