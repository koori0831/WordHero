using System.Collections.Generic;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.ProgressRate.Code
{
    /// <summary>
    /// 스테이지 진행도 맵 표시 요청 이벤트 (명시적 요청용)
    /// </summary>
    public readonly record struct ShowStageProgressMapEvent(int CurrentStageIndex, List<DoorType> StageTypes) : IEvent;

    /// <summary>
    /// 스테이지 진행도 맵 연출 완료 및 닫힘 이벤트
    /// </summary>
    public readonly record struct StageProgressMapClosedEvent : IEvent;

    /// <summary>
    /// 스테이지 진행도 카운트 리셋 이벤트
    /// </summary>
    public readonly record struct ResetStageProgressEvent : IEvent;
}
