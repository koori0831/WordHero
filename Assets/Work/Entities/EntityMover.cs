using UnityEngine;
using Work.StatSystem.Code;

namespace Code.Entities
{
    [RequireComponent(typeof(CharacterController))]
    public class EntityMover : MonoBehaviour, IEntityComponent, IAfterInitCompo
    {
        [SerializeField] private StatSO statSO;
        [SerializeField] private float mass = 3.0f;
        [SerializeField] private float drag = 5.0f;

        private Entity _owner;
        private EntityStatCompo _stat;
        private StatSO _speedStat;
        private CharacterController _controller;
        private Transform _camTransform;

        private Vector3 _impactVelocity = Vector3.zero;

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
            _speedStat = _stat.GetStat(statSO);
        }

        private void Update()
        {
            ApplyImpact();
        }

        public void Move(Vector2 direction)
        {
            Vector3 camForward = Vector3.Scale(_camTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = _camTransform.right;

            Vector3 move = (camForward * direction.y + camRight * direction.x) * Speed * Time.deltaTime;

            _controller.Move(move);

            if (direction.sqrMagnitude > 0.01f)
            {
                Vector3 lookDirection = new Vector3(move.x, 0, move.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
                }
            }
        }

        private void ApplyImpact()
        {
            if (_impactVelocity.magnitude > 0.1f)
            {
                _controller.Move(_impactVelocity * Time.deltaTime);
            }

            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, drag * Time.deltaTime);
        }

        public void AddImpulse(Vector2 force)
        {
            Vector3 impulse = new Vector3(force.x, 0, force.y);

            _impactVelocity += impulse / mass;
        }

        public Vector2 GetVelocity()
        {
            return new Vector2(_controller.velocity.x, _controller.velocity.z);
        }
    }
}