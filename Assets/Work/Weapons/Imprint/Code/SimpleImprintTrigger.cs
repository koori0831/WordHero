using System;
using Work.Core.Utils.EventBus;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code
{
    [Serializable]
    public abstract record SimpleImprintTrigger<TEvent> : IImprintTriggerEvent
        where TEvent : IImprintTriggerEvent
    {
        public Action Subscribe(Action onTriggered)
        {
            void Handler(TEvent _)
            {
                onTriggered?.Invoke();
            }

            Bus<TEvent>.Events += Handler;
            return () => Bus<TEvent>.Events -= Handler;
        }
    }
}