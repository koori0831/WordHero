using Work.Core.Utils.EventBus;

namespace Work.ETC.LocationUI.Code
{
    /// <summary>
    /// 로케이션 이름 표시 이벤트
    /// </summary>
    public readonly record struct OnShowLocationNameEvent(string LocationName) : IEvent;

    /// <summary>
    /// 로케이션 UI 재생 이벤트
    /// </summary>
    public readonly record struct PlayLocationUIEvent : IEvent;
}
