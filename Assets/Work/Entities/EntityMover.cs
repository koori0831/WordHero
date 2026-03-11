using UnityEngine;
using Work.Combat.Code;
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
            Vector3 lookDirection = (camForward * direction.y + camRight * direction.x);

            if (direction.sqrMagnitude > 0.01f && lookDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        public void ApplyRootMotion(Vector3 deltaPosition, bool useXPos = false)
        {
            Vector3 motion = new Vector3(deltaPosition.x, 0f, deltaPosition.z);

            if (!useXPos)
            {
                Vector3 forward = transform.forward;

                // 투영으로 빗겨 나가는 움직임을 제외한 직진 벡터만 남긴다.
                motion = Vector3.Project(motion, forward);
            }

            if (motion.sqrMagnitude > 0.0001f) // 부동 소수점 오차를 고려해 약간의 여유를 둠
            {
                _controller.Move(motion);
            }
        }

        public async void KnockBack(KnockbackData knockbackData)
        {

            _owner.transform.rotation = Quaternion.LookRotation(new Vector3(-knockbackData.Direction.x, 0, -knockbackData.Direction.z));

            float duration = knockbackData.Duration;
            Vector3 direction = knockbackData.Direction.normalized;
            direction.y = 0; 
            float currentTime = 0;
            float maxSpeed = knockbackData.Force;
            AnimationCurve moveCurve = knockbackData.KnockbackCurve;

            while (currentTime < duration)
            {
                float normalizeTime = currentTime / duration;
                float currentSpeed = maxSpeed * moveCurve.Evaluate(normalizeTime);
                Vector3 currentMovement = direction * currentSpeed;
                _owner.transform.Translate(currentMovement * Time.fixedDeltaTime, Space.World);
                currentTime += Time.fixedDeltaTime;
                await Awaitable.FixedUpdateAsync();
            }

        }
    }
}
