using UnityEngine;
using Work.Combat.Code;
using Work.Enemies.Code;

namespace Work.Agents.Code
{
    public class AgentMovementModule : MonoBehaviour, IAgentModule
    {
        private Agent _owner;

        public virtual void Initialize(Agent agent)
        {
            _owner = agent;
        }

        public virtual async void KnockBack(KnockbackData knockbackData)
        {
            //넉백에 대한 면역이 있는지 체크하는건 EnemyKnockbackModule에서 하자.
            float duration = knockbackData.Duration;
            Vector3 direction = knockbackData.Direction.normalized;
            direction.y = 0; // 수평 방향으로만 넉백이 적용되도록 y축은 제거
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
            //여기서 추가 작업을 안해주면 넉백이 이상해진다. 일단 이상하게 해서 봅시다.
        }
    }
}
