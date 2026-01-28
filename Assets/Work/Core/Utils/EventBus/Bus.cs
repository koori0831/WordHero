using System;

namespace Work.Core.Utils.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        public static Action<T> Events;

        // 추적 전용 이벤트
        public static Action<string> OnLog;

        public static void Raise(T evt)
        {
            Events?.Invoke(evt);
            // record의 ToString() 덕분에 데이터가 문자열로 깔끔하게 넘어갑니다.
            OnLog?.Invoke(evt.ToString());
        }
    }
}
