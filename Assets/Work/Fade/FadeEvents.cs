using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Fade
{
    public readonly record struct OnFadeEvent(bool isFadeIn = true) : IEvent;
    public readonly record struct OnFadeCompletedEvent(bool isFadeIn = true) : IEvent;
}