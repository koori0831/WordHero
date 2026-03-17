using Code.FSM;
using UnityEngine;
using Work.Agents.Code;

namespace Work.Players.Code
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationModule : MonoBehaviour, IAgentAnimationModule
    {
        private Agent _owner;
        private Animator _animator;
        private AgentStateModule _stateModule;
        private PlayerMovementModule _movementModule;

        public void Initialize(Agent agent)
        {
            _owner = agent;
            _animator = GetComponent<Animator>();
            _stateModule = _owner.GetModule<AgentStateModule>(true);
            _movementModule = _owner.GetModule<PlayerMovementModule>(true);
        }

        public void SetParam(int animHash, float value) => _animator.SetFloat(animHash, value);
        public void SetParam(int animHash, int value) => _animator.SetInteger(animHash, value);
        public void SetParam(int animHash, bool value) => _animator.SetBool(animHash, value);
        public void SetTrigger(int animHash) => _animator.SetTrigger(animHash);

        public void SetApplyRootMotion(bool apply)
        {
            _animator.applyRootMotion = apply;
        }

        public float GetStateLength(int layer = 0)
        {
            if (_animator == null) return 0f;

            if (_animator.IsInTransition(layer))
            {
                AnimatorStateInfo nextInfo = _animator.GetNextAnimatorStateInfo(layer);
                if (nextInfo.length > 0f)
                {
                    return nextInfo.length;
                }
            }

            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(layer);
            if (info.length > 0f)
            {
                return info.length;
            }

            AnimatorClipInfo[] clips = _animator.GetCurrentAnimatorClipInfo(layer);
            if (clips.Length > 0 && clips[0].clip != null)
            {
                return clips[0].clip.length;
            }

            return 0f;
        }

        private void OnAnimatorMove()
        {
            if (!_animator.applyRootMotion)
            {
                return;
            }

            if (_movementModule != null)
            {
                _movementModule.ApplyRootMotion(_animator.deltaPosition);
            }
            else
            {
                _owner.transform.position += _animator.deltaPosition;
            }

            if (_animator.deltaRotation != Quaternion.identity)
            {
                _owner.transform.rotation *= _animator.deltaRotation;
            }

            if (_owner.transform != transform)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }

        public void AnimationEvent(AnimationEventType eventType)
        {
            _stateModule?.TriggerEvent(eventType);
        }
    }
}
