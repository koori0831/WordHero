using Code.FSM;
using UnityEngine;

namespace Code.Entities
{
    public enum AnimationEventType
    {
        End,
        Attack,
        ComboWindowOpen,
        ComboWindowClose,
    }

    [RequireComponent (typeof(Animator))]
    public class EntityAnimator : MonoBehaviour, IEntityComponent
    {
        private Entity _owner;
        private Animator _animator;
        private StateCompo _stateCompo;
        private EntityMover _mover;
        private Transform _ownerTransform;

        public Entity Owner => _owner;

        public void InitCompo(Entity entity)
        {
            _owner = entity;
            _animator = GetComponent<Animator>();
            _stateCompo = entity.GetCompo<StateCompo>();
            _mover = entity.GetCompo<EntityMover>(true);
            _ownerTransform = entity.transform;
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

        public void SetApplyRootMotion(bool apply)
        {
            _animator.applyRootMotion = apply;
        }

        public float GetStateLength(int layer = 0)
        {
            if (_animator == null)
                return 0f;

            if (_animator.IsInTransition(layer))
            {
                var nextInfo = _animator.GetNextAnimatorStateInfo(layer);
                if (nextInfo.length > 0f)
                    return nextInfo.length;
            }

            var info = _animator.GetCurrentAnimatorStateInfo(layer);
            if (info.length > 0f)
                return info.length;

            var clips = _animator.GetCurrentAnimatorClipInfo(layer);
            if (clips.Length > 0 && clips[0].clip != null)
                return clips[0].clip.length;

            return 0f;
        }

        private void OnAnimatorMove()
        {
            if (!_animator.applyRootMotion)
                return;

            if (_mover != null)
            {
                _mover.ApplyRootMotion(_animator.deltaPosition);
            }
            else
            {
                _ownerTransform.position += _animator.deltaPosition;
            }

            if (_animator.deltaRotation != Quaternion.identity)
            {
                _ownerTransform.rotation *= _animator.deltaRotation;
            }

            if (_ownerTransform != transform)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }

        public void AnimationEvent(AnimationEventType eventType)
        {
            _stateCompo.TriggerEvent(eventType);
        }
    }
}
