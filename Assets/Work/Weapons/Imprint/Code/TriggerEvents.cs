using System;
using Work.Core.Utils.EventBus;

namespace Work.Weapons.Imprint.Code
{
    [Serializable]
    public sealed record OnDodgeSuccessTrigger : SimpleImprintTrigger<OnDodgeSuccessTrigger>;

    [Serializable]
    public sealed record OnHitSuccessTrigger : SimpleImprintTrigger<OnHitSuccessTrigger>;

    [Serializable]
    public sealed record OnTookDamageTrigger : SimpleImprintTrigger<OnTookDamageTrigger>;

    public readonly record struct WeaponTriggerOpenedEvent(float Duration) : IEvent;
    public readonly record struct WeaponTriggerActivatedEvent : IEvent;
}
