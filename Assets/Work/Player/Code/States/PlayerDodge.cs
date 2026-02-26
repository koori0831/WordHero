using Code.Entities;
using Code.FSM;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Player.Code.States
{
    public readonly record struct DodgeEvent : IEvent;

    public class PlayerDodge : PlayerStates
    {
        private EntityHealth _health;
        private PlayerInputRoot _inputRoot;
        private EntityMover _mover;

        public PlayerDodge(StateMachine stateMachine, Entity entity, int animationHash) : base(stateMachine, entity, animationHash)
        {
            _health = _entity.GetCompo<EntityHealth>();
            _inputRoot = _entity.GetCompo<PlayerInputRoot>();
            _mover = _entity.GetCompo<EntityMover>();
        }

        public override void Enter()
        {
            base.Enter();

            _mover.Move(_inputRoot.MoveVector, false); 

            _health.IsDamageImmune = true;
            _animator.SetApplyRootMotion(true);
            Bus<DodgeEvent>.Raise(new DodgeEvent());
        }

        public override void Exit()
        {
            _health.IsDamageImmune = false;
            _animator.SetApplyRootMotion(false);
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
