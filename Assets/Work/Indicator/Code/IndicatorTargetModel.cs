using UnityEngine;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 인디케이터 추적 대상 데이터
    /// </summary>
    public readonly struct IndicatorTargetModel
    {
        /// <summary>
        /// 추적 대상 적
        /// </summary>
        public readonly Enemy TargetEnemy;

        /// <summary>
        /// 추적 대상 트랜스폼
        /// </summary>
        public readonly Transform TargetTransform;

        /// <summary>
        /// 인디케이터 추적 대상 데이터 생성
        /// </summary>
        public IndicatorTargetModel(Enemy targetEnemy)
        {
            TargetEnemy = targetEnemy;
            TargetTransform = targetEnemy != null ? targetEnemy.transform : null;
        }

        /// <summary>
        /// 추적 대상 유효 여부
        /// </summary>
        public bool IsValid => TargetEnemy != null && TargetTransform != null && TargetEnemy.IsDead == false;
    }
}
