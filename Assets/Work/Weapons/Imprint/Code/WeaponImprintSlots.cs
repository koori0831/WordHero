using System.Collections.Generic;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code
{
    public class WeaponImprintSlots
    {
        [SerializeField] private ImprintWordSO attack;
        [SerializeField] private ImprintWordSO effect;
        [SerializeField] private ImprintWordSO stat;

        public ImprintWordSO Attack => attack;
        public ImprintWordSO Effect => effect;
        public ImprintWordSO Stat => stat;

        public bool HasAnyImprint()
        {
            return attack != null || effect != null || stat != null;
        }

        public ImprintWordSO Get(ImprintType type)
        {
            return type switch
            {
                ImprintType.Attack => attack,
                ImprintType.Effect => effect,
                ImprintType.Stat => stat,
                _ => null
            };
        }

        public bool TryEquip(ImprintWordSO imprint)
        {
            if (imprint == null) return false;

            switch (imprint.Type)
            {
                case ImprintType.Attack:
                    attack = imprint;
                    return true;

                case ImprintType.Effect:
                    effect = imprint;
                    return true;

                case ImprintType.Stat:
                    stat = imprint;
                    return true;
            }

            return false;
        }

        public void Clear(ImprintType type)
        {
            switch (type)
            {
                case ImprintType.Attack:
                    attack = null;
                    break;
                case ImprintType.Effect:
                    effect = null;
                    break;
                case ImprintType.Stat:
                    stat = null;
                    break;
            }
        }

        public IEnumerable<ImprintWordSO> EnumerateInOrder()
        {
            if (stat != null) yield return stat;
            if (effect != null) yield return effect;
            if (attack != null) yield return attack;
        }
    }
}
