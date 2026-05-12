using System;
using R3;

namespace Work.Core.Utils.EventBus
{
    /// <summary>
    /// 이벤트 버스 R3 어댑터
    /// </summary>
    public static class BusObservable
    {
        /// <summary>
        /// 이벤트 스트림
        /// </summary>
        public static Observable<T> On<T>() where T : IEvent
        {
            return Observable.Create<T>(observer =>
            {
                Action<T> handler = observer.OnNext;
                Bus<T>.Events += handler;

                return Disposable.Create(() => Bus<T>.Events -= handler);
            });
        }
    }
}
