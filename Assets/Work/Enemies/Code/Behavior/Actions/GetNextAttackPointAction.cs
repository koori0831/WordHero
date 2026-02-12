using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetNextAttackPoint", story: "[Self] get next attack [point]", category: "Action", id: "b5e4baf2e54aaa3400e9a1fe00ac402c")]
public partial class GetNextAttackPointAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<Vector3> Point;
    [SerializeReference] public BlackboardVariable<float> AttackRange;
    [SerializeReference] public BlackboardVariable<float> DistanceToKeepRange;
    [SerializeReference] public BlackboardVariable<float> MoveDistance;

    //지금 거리가 너무 가까우면 해당 자리에서 계속 공격하는 문제가 있다.
    //거리가 어느정도 가까워서 저 함수로 답이 도출되지 않으면 
    //멀어져야하는 거리에서 플레이어와의 거리를 빼고 
    //뺀 값에다가 offset으로 작은 수를 더해서 움직여야하는 거리를 얻고 
    //나에서 타겟으로 향하는 반대 방향으로 해당 거리를 곱한다. 
    //그렇게해서 위치를 얻는다.

    protected override Status OnStart()
    {
        Vector3 enemyPos = Self.Value.transform.position;
        Vector3 targetPos = Target.Value.position;
        Vector3 newPos = GetSmartRandomPosition(enemyPos, targetPos, MoveDistance, AttackRange, DistanceToKeepRange, enemyPos.y);

        Point.Value = newPos;

        if (newPos == enemyPos)
            return Status.Failure;

        return Status.Success;
    }

    Vector3 GetSmartRandomPosition(
    Vector3 enemyPos,
    Vector3 playerPos,
    float r1,   // 적 사거리
    float r2,   // 플레이어 기준 최대 거리
    float r3,   // 플레이어 기준 최소 거리
    float fixedY
)
    {
        // XZ 평면으로 투영
        Vector2 E = new Vector2(enemyPos.x, enemyPos.z);
        Vector2 P = new Vector2(playerPos.x, playerPos.z);

        float d = Vector2.Distance(E, P);

        // 안전 체크 (설계상 파란 영역이 존재해야 함)
        if (d > r1 + r2) //가까워져야하는 조건이므로 냅두면 알아서 가까워짐
        {
            // 이 경우는 AI 상태 전환이 맞음 (접근 / 후퇴 등)
            return enemyPos;
        }

        if (d <= 0.0001f || d < Mathf.Abs(r1 - r3)) //멀어져야 하니까 거리 계산해서 먼곳으로 좌표찍어줘야함
        {
            float range = d + r3 + 1f;
            Vector2 direction = E - P;
            direction.Normalize();
            direction *= range;
            return new Vector3(E.x + direction.x, fixedY, E.y + direction.y);
        }

        // 적 → 플레이어 방향
        Vector2 dir = (P - E).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x);

        // 각도 계산 (외곽 / 내곽)
        float angleOuter = Mathf.Acos(
            Mathf.Clamp((d * d + r1 * r1 - r2 * r2) / (2f * d * r1), -1f, 1f)
        );

        float angleInner = Mathf.Acos(
            Mathf.Clamp((d * d + r1 * r1 - r3 * r3) / (2f * d * r1), -1f, 1f)
        );

        // 좌 / 우 랜덤 선택
        float sign = Random.value < 0.5f ? -1f : 1f;

        float angle = baseAngle + sign * Random.Range(angleInner, angleOuter);

        // 반지름 범위 (적 기준)
        float minR = Mathf.Max(0f, d - r2);
        float maxR = Mathf.Min(r1, d - r3);

        float radius = Random.Range(minR, maxR);

        // XZ 평면 좌표 계산
        Vector2 pos2D = E + new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ) * radius;

        return new Vector3(pos2D.x, fixedY, pos2D.y);
    }
}

