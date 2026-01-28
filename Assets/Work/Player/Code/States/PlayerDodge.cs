using Code.FSM;
using Code.Entities;
using UnityEngine;
using Work.StatSystem.Code;

namespace Work.Player.Code.States
{
    public class PlayerDodge : PlayerStates
    {
        private EntityMover _mover;
        private EntityStatCompo _statCompo;
        private EntityHealth _health;

        private StatSO _dodgePowerStat;

        public PlayerDodge(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            _mover = _entity.GetCompo<EntityMover>();
            _statCompo = _entity.GetCompo<EntityStatCompo>();
            _health = _entity.GetCompo<EntityHealth>();

            _statCompo.TryGetStat("DodgePower", out _dodgePowerStat);
            Debug.Assert(_dodgePowerStat != null, "DodgePower stat not found on entity.");
        }

        public override void Enter()
        {
            base.Enter();

            _health.IsDamageImmune = true;
            Vector3 dodgeDirection = -_entity.transform.forward;
            _mover.AddImpulse(dodgeDirection * _dodgePowerStat.Value);
        }

        public override void Exit()
        {
            _health.IsDamageImmune = false;
            base.Exit();
        }

        public override void OnTriggerEnter(AnimationEventType eventType)
        {
            base.OnTriggerEnter(eventType);
            if (eventType == AnimationEventType.End)
            {
                _stateMachine.ChangeState("Idle");
            }
        }
    }
}