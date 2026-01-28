using UnityEngine;
using System.Collections;

namespace Code.Entities
{
    [RequireComponent(typeof(CharacterController))]
    public class EntityMover : MonoBehaviour, IEntityComponent
    {
        [field:SerializeField] public float Speed { get; private set; } = 5f;
        private Entity _owner;
        private CharacterController _controller;

        private Transform _camTransform;

        public Entity Owner => _owner;

        public void InitCompo(Entity entity)
        {
            _owner = entity;
            _controller = GetComponent<CharacterController>();

            _camTransform = Camera.main.transform;
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
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.05f);
            }
        }

        public void AddImpulse(Vector2 force)
        {
            Vector3 impulse = new Vector3(force.x, 0, force.y);
            _controller.Move(impulse * Time.deltaTime);
        }

        public Vector2 GetVelocity()
        {
            return new Vector2(_controller.velocity.x, _controller.velocity.z);
        }
    }
}