using System;

namespace Work.Core.Utils.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        public static Action<T> Events;

        public static void Raise(T evt) => Events?.Invoke(evt);
    }
}
