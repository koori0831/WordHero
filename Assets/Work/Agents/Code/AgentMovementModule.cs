using UnityEngine;
using Work.Combat.Code;

namespace Work.Agents.Code
{
    public class AgentMovementModule : MonoBehaviour, IAgentModule
    {
        [SerializeField] protected LayerMask groundLayerMask;
        protected Agent _owner;


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


            Vector3 endPoint = _owner.transform.position + direction * maxSpeed; // 넉백이 끝나는 지점 계산
            Ray ray = new Ray(_owner.transform.position + Vector3.up * 0.5f, direction);

            // 넉백으로 인해서 밀려나는 동안에 장애물과 충돌하는지 체크
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxSpeed, groundLayerMask))
            {
                endPoint = hitInfo.point; // 충돌 지점으로 넉백 끝나는 지점 수정
                //해당 지점을 지나치게 넉백이 적용되는 것을 방지하기 위해서 넉백의 최대 속도를 조정
                float distanceToObstacle = Vector3.Distance(_owner.transform.position, endPoint);
                if (distanceToObstacle < maxSpeed)
                {
                    maxSpeed = distanceToObstacle; // 장애물까지의 거리가 넉백 최대 속도보다 짧으면, 최대 속도를 장애물까지의 거리로 조정
                }
            }


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
