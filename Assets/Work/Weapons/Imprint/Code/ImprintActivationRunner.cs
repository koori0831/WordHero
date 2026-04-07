using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code
{
    public class ImprintActivationRunner
    {
        public void Activate(WeaponRuntimeInstance runtimeWeapon, SkillContext context)
        {
            if (runtimeWeapon == null || runtimeWeapon.Imprints == null)
                return;

            foreach (var imprint in runtimeWeapon.Imprints.EnumerateInOrder())
            {
                if (imprint == null || imprint.Effects == null)
                    continue;

                foreach (var effect in imprint.Effects)
                {
                    if (effect == null)
                        continue;

                    effect.ExecuteEffect(context);
                }
            }
        }
    }
}
