using UnityEngine;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 인디케이터 표시 상태 데이터
    /// </summary>
    public readonly struct IndicatorViewState
    {
        /// <summary>
        /// 표시 여부
        /// </summary>
        public readonly bool IsVisible;

        /// <summary>
        /// UI 위치
        /// </summary>
        public readonly Vector2 AnchoredPosition;

        /// <summary>
        /// UI 회전값
        /// </summary>
        public readonly Quaternion Rotation;

        /// <summary>
        /// 인디케이터 표시 상태 데이터 생성
        /// </summary>
        public IndicatorViewState(bool isVisible, Vector2 anchoredPosition, Quaternion rotation)
        {
            IsVisible = isVisible;
            AnchoredPosition = anchoredPosition;
            Rotation = rotation;
        }
    }
}
