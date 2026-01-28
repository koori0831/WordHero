using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public readonly record struct InputEnableEvent(bool Enable) : IEvent;

    public readonly record struct InputInteractEvent : IEvent;
    public readonly record struct InputMenuEvent : IEvent;
    public readonly record struct InputPaletteSwapEvent : IEvent;
    public readonly record struct InputSpellAEvent : IEvent;
    public readonly record struct InputSpellBEvent : IEvent;

    public readonly record struct InputWordSellectLeftEvent(bool IsRelease) : IEvent;
    public readonly record struct InputWordSellectRightEvent(bool IsRelease) : IEvent;
    public readonly record struct InputWordSellectUpEvent(bool IsRelease) : IEvent;
    public readonly record struct InputWordSellectDownEvent(bool IsRelease) : IEvent;

    public readonly record struct InputWordCancleEvent : IEvent;
}