using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public readonly record struct InputEnableEvent(bool Enable) : IEvent;

    public readonly record struct InputInteractEvent : IEvent;
    public readonly record struct InputMenuEvent : IEvent;

    public readonly record struct InputDodgeEvent : IEvent;
    public readonly record struct InputAttackEvent : IEvent;
}