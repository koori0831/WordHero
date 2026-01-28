using System;

namespace Work.Core.Utils.EventBus
{
    public interface IEventTracker
    {
        void Subscribe(Action<string> onEventRaised);
    }
}