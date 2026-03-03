using UnityEngine;
using Work.StatSystem.Code;

namespace Code.Entities
{
    [RequireComponent(typeof(CharacterController))]
    public class EntityMover : MonoBehaviour, IEntityComponent, IAfterInitCompo
    {
        private Entity _owner;
        private EntityStatCompo _stat;
        private StatSO _speedStat;
        private CharacterController _controller;
        private Transform _camTransform;

        public float Speed => _speedStat.Value;
        public Entity Owner => _owner;

        public void InitCompo(Entity entity)
        {
            _owner = entity;
            _controller = GetComponent<CharacterController>();
            _stat = entity.GetCompo<EntityStatCompo>();
            _camTransform = Camera.main.transform;
        }

        public void AfterInit()
        {
            _stat.TryGetStat("MoveSpeed", out _speedStat);
        }

        public void Move(Vector2 direction, bool isSmooth = true)
        {
            Vector3 camForward = Vector3.Scale(_camTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = _camTransform.right;

            Vector3 move = (camForward * direction.y + camRight * direction.x) * Time.deltaTime;

            if (direction.sqrMagnitude > 0.01f)
            {
                Vector3 lookDirection = new Vector3(move.x, 0, move.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    if (isSmooth)
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
                    else
                        transform.rotation = targetRotation;
                }
            }
        }

        public void ApplyRootMotion(Vector3 deltaPosition)
        {
            Vector3 motion = new Vector3(deltaPosition.x, 0f, deltaPosition.z);
            if (motion.sqrMagnitude > 0f)
            {
                _controller.Move(motion);
            }
        }
    }
}
