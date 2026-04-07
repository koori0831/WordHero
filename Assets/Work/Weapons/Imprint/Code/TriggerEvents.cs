using System;

namespace Work.Weapons.Imprint.Code
{
    [Serializable]
    public sealed record OnDodgeSuccessTrigger : SimpleImprintTrigger<OnDodgeSuccessTrigger>;

    [Serializable]
    public sealed record OnHitSuccessTrigger : SimpleImprintTrigger<OnHitSuccessTrigger>;

    [Serializable]
    public sealed record OnTookDamageTrigger : SimpleImprintTrigger<OnTookDamageTrigger>;
}