using Alchemy.Inspector;
using System;
using UnityEngine;

namespace Work.Combat.Code
{

    public enum StatusType
    {
        // =========================
        // 1. Control (행동 제한 계열) 수치필요 X , 틱당 효과 필요 X
        // =========================

        [InspectorName("피격 경직")] Hit = 101,             // 피격 경직 (짧은 히트 리액션, 행동 약간 끊김)
        [InspectorName("스턴")] Stun = 102,                 // 경직(스턴) - 일정 시간 모든 행동 불가
        [InspectorName("넉백")] Knockback = 103,            // 넉백 - 뒤로 밀려남
        [InspectorName("다운")] Knockdown = 104,            // 다운 - 넘어져서 일정 시간 행동 불가
        [InspectorName("잡힘")] Grabbed = 105,              // 잡힘 - 특정 스킬/몬스터에게 구속된 상태
        [InspectorName("침묵")] Silence = 106,              // 침묵 - 스킬 사용 불가 (평타는 가능하게 설계 가능)

        // =========================
        // 2. Immunity (면역/저항 계열) 수치필요 X , 틱당 효과 필요 X
        // =========================

        [InspectorName("피격 면역")] HitImmunity = 201,        // 피격 면역 - 맞아도 히트 리액션 없음
        [InspectorName("슈퍼아머")] SuperArmor = 202,         // 경직 면역 - 데미지는 받지만 스턴/경직 무효
        [InspectorName("완전 무적")] Invincible = 203,         // 완전 무적 - 모든 데미지 무효
        [InspectorName("CC 면역")] CrowdControlImmune = 204,    // CC 면역 - 스턴, 빙결 등 제어 효과 무효
        [InspectorName("화염 면역")] FireImmune = 205,            // 화염 면역 - Burn 등 화염 계열 무효
        [InspectorName("디버프 면역")] DebuffImmune = 206,       // 디버프 면역 - 상태이상 전체 또는 일부 무효
        [InspectorName("넉백 면역")] KnockbackImmune = 207,    // 넉백 면역 - 밀리지 않음

        // =========================
        // 3. DoT (지속 피해 계열) 수치필요 O , 틱당 효과 필요 O
        // =========================

        [InspectorName("화상")] Burn = 301,               // 화상 - 일정 시간 동안 지속 화염 피해
        [InspectorName("중독")] Poison = 302,             // 중독 - 일정 시간 지속 독 피해
        [InspectorName("출혈")] Bleed = 303,              // 출혈 - 체력 비례 또는 고정 지속 피해
        [InspectorName("감전")] Shock = 304,              // 감전 - 지속 전기 피해 + 추가 효과 가능
        [InspectorName("빙결")] Freeze = 305,              // 감전 - 지속 전기 피해 + 추가 효과 가능

        // =========================
        // 4. Crowd Control (행동 변화 계열) 수치필요 X , 틱당 효과 필요 X
        // =========================

        [InspectorName("동상")] Frostbite = 401,          // 동상 - 이동/공속 감소
        [InspectorName("슬로우")] Slow = 402,             // 슬로우 - 이동 속도 감소
        [InspectorName("공격 속도 감소")] AttackSpeedDown = 403,    // 공격 속도 감소
        [InspectorName("혼란")] Confuse = 404,            // 혼란 - 랜덤 이동 또는 타겟 혼동
        [InspectorName("공포")] Fear = 405,               // 공포 - 플레이어 반대 방향으로 도망
        [InspectorName("실명")] Blind = 406,              // 실명 - 명중률 감소 또는 시야 감소

        // =========================
        // 5. Buff (강화 계열) 수치필요 O , 틱당 효과 필요 X
        // =========================

        [InspectorName("공격력 증가")] AttackUp = 501,           // 공격력 증가
        [InspectorName("방어력 증가")] DefenseUp = 502,          // 방어력 증가
        [InspectorName("이동 속도 증가")] SpeedUp = 503,            // 이동 속도 증가
        [InspectorName("흡혈")] LifeSteal = 504,          // 흡혈 - 공격 시 체력 회복
        [InspectorName("광폭화")] Enrage = 505,             // 광폭화 - 공격력 증가 + 제어 저항 증가 등
        [InspectorName("보호막")] Shield = 506              // 보호막 - 추가 체력 버퍼
    }

    [Serializable]
    public class StatusEffect
    {
        public bool IsTickEffect => (int)type / 100 == 3; // DoT 계열인지 여부
        public bool IsValueEffect => (int)type / 100 == 5 || (int)type / 100 == 3; //DoT, Buff 계열인지 여부

        public StatusType type;
        public bool isInfinite; // 무한 지속 여부
        private bool isDuration => !isInfinite; // 지속 시간 필요 여부
        [ShowIf(nameof(isDuration))] public float Duration; // 지속 시간 (초)
        [ShowIf(nameof(IsTickEffect))] public float TickInterval;
        [ShowIf(nameof(IsValueEffect))] public float Value;

        [HideInInspector] public float timer; // 내부 타이머 (틱 간격 또는 지속 시간 계산용)
        [HideInInspector] public float tickTimer; // 내부 타이머 (틱 간격 또는 지속 시간 계산용)
    }
}
