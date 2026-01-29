using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Fade
{
    public record struct OnFadeEvent(bool isFadeIn = true) : IEvent;
    public record struct OnFadeCompletedEvent(bool isFadeIn = true) : IEvent;
}