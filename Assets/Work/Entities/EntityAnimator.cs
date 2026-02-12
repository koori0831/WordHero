using Code.FSM;
using UnityEngine;

namespace Code.Entities
{
    public enum AnimationEventType
    {
        End,
        Attack,
    }

    [RequireComponent (typeof(Animator))]
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        private Entity _owner;
        private Animator _animator;
        private StateCompo _stateCompo;

        public Entity Owner => _owner;

        public void InitCompo(Entity entity)
        {
            _owner = entity;
            _animator = GetComponent<Animator>();
            _stateCompo = entity.GetCompo<StateCompo>();
        }

        public void SetParam(int animHash, float value)
        {
            _animator.SetFloat(animHash, value);
        }
        public void SetParam(int animHash, int value)
        {
            _animator.SetInteger(animHash, value);
        }
        public void SetParam(int animHash, bool value)
        {
            _animator.SetBool(animHash, value);
        }
        public void SetTrigger(int animHash)
        {
            _animator.SetTrigger(animHash);
        }

        public void AnimationEvent(AnimationEventType eventType)
        {
            _stateCompo.TriggerEvent(eventType);
        }
    }
}