using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Player.Code
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementModule : AgentMovementModule
    {
        [SerializeField] private float baseMoveSpeed = 5f;

        private CharacterController _controller;
        private Transform _camTransform;
        private AgentStatusModule _statusModule;

        public float Speed
        {
            get
            {
                float speedUp = _statusModule != null ? _statusModule.GetStatusValue(StatusType.SpeedUp) : 0f;
                return baseMoveSpeed * (1f + speedUp);
            }
        }

        public override void Initialize(Agent agent)
        {
            _owner = agent;
            _controller = GetComponent<CharacterController>();
            _camTransform = Camera.main != null ? Camera.main.transform : transform;
            _statusModule = _owner.GetModule<AgentStatusModule>(true);
        }

        public void Move(Vector2 direction, bool isSmooth = true)
        {
            Vector3 camForward = Vector3.Scale(_camTransform.forward, new Vector3(1f, 0f, 1f)).normalized;
            Vector3 camRight = _camTransform.right;
            Vector3 lookDirection = camForward * direction.y + camRight * direction.x;

            if (direction.sqrMagnitude > 0.01f && lookDirection != Vector3.zero)
            {
                _owner.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        public void ApplyRootMotion(Vector3 deltaPosition, bool useXPos = false)
        {
            Vector3 motion = new Vector3(deltaPosition.x, 0f, deltaPosition.z);

            if (!useXPos)
            {
                motion = Vector3.Project(motion, transform.forward);
            }

            if (motion.sqrMagnitude > 0.0001f)
            {
                _controller.Move(motion);
            }
        }

        public override async void KnockBack(KnockbackData knockbackData)
        {
            _owner.transform.rotation = Quaternion.LookRotation(new Vector3(-knockbackData.Direction.x, 0f, -knockbackData.Direction.z));

            float duration = knockbackData.Duration;
            Vector3 direction = knockbackData.Direction.normalized;
            direction.y = 0f;

            float currentTime = 0f;
            float maxSpeed = knockbackData.Force;
            AnimationCurve moveCurve = knockbackData.KnockbackCurve;

            while (currentTime < duration)
            {
                float normalizedTime = currentTime / duration;
                float currentSpeed = maxSpeed * moveCurve.Evaluate(normalizedTime);
                Vector3 currentMovement = direction * currentSpeed;
                _owner.transform.Translate(currentMovement * Time.fixedDeltaTime, Space.World);
                currentTime += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }
        }
    }
}
