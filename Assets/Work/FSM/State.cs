using Code.Entities;

namespace Code.FSM
{
    public class State
    {
        protected StateMachine _stateMachine;
        protected Entity _entity;
        protected EntityAnimator _animator;
        protected int _animationHash;

        public State(StateMachine stateMachine, Entity entity, int animationHash)
        {
            _stateMachine = stateMachine;
            _entity = entity;
            _animator = _entity.GetCompo<EntityAnimator>();
            _animationHash = animationHash;

        }

        public virtual void Enter()
        {
            if (_animationHash != 0)
            {
                _animator.SetParam(_animationHash, true);
            }
        }

        public virtual void Exit()
        {
            if (_animationHash != 0)
            {
                _animator.SetParam(_animationHash, false);
            }
        }

        public virtual void Update()
        {
        }

        public virtual void OnTriggerEnter(AnimationEventType eventType)
        {
        }
    }
}
