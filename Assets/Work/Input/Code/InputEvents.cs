using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public struct InputEnableEvent : IEvent
    {
        public readonly bool Enable;
        public InputEnableEvent(bool enable)
        {
            Enable = enable;
        }
    }

    public struct InputInteractEvent : IEvent
    {
    }
    public struct InputMenuEvent : IEvent
    {
    }
    public struct InputPaletteSwapEvent : IEvent
    {
    }
    public struct InputSpellAEvent : IEvent
    {
    }
    public struct InputSpellBEvent : IEvent
    {
    }
    public struct InputWordSellectLeftEvent : IEvent
    {
        public readonly bool IsRelease;
        public InputWordSellectLeftEvent(bool isRelease)
        {
            IsRelease = isRelease;
        }
    }
    public struct InputWordSellectRightEvent : IEvent
    {
        public readonly bool IsRelease;
        public InputWordSellectRightEvent(bool isRelease)
        {
            IsRelease = isRelease;
        }
    }
    public struct InputWordSellectUpEvent : IEvent
    {
        public readonly bool IsRelease;
        public InputWordSellectUpEvent(bool isRelease)
        {
            IsRelease = isRelease;
        }
    }
    public struct InputWordSellectDownEvent : IEvent
    {
        public readonly bool IsRelease;
        public InputWordSellectDownEvent(bool isRelease)
        {
            IsRelease = isRelease;
        }
    }
    public struct InputWordCancleEvent : IEvent
    {
    }
}