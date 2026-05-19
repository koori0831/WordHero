using System.Collections.Generic;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;

namespace Work.ProgressRate.Code
{
    /// <summary>
    /// 게임 최초 시작 시 전체 맵 프리뷰 연출 요청 이벤트
    /// </summary>
    public readonly record struct PlayInitialProgressMapEvent : IEvent;

    /// <summary>
    /// 스테이지 진행도 맵 연출 완료 및 닫힘 이벤트
    /// </summary>
    public readonly record struct StageProgressMapClosedEvent : IEvent;

    /// <summary>
    /// 스테이지 진행도 카운트 리셋 이벤트
    /// </summary>
    public readonly record struct ResetStageProgressEvent(DoorType InitialRoomType = DoorType.Wood) : IEvent;

    /// <summary>
    /// 스테이지 진행도 초기 방 타입 강제 설정 이벤트
    /// </summary>
    public readonly record struct SetInitialStageProgressEvent(DoorType InitialRoomType) : IEvent;
}
