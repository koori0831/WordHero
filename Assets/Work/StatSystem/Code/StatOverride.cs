using Alchemy.Inspector;
using System;
using UnityEngine;

namespace Work.StatSystem.Code
{
    [Serializable]
    public class StatOverride
    {
        public StatSO stat;
        [SerializeField] private bool isUseOverride = true;
        [ShowIf(nameof(isUseOverride))]
        [SerializeField] private float overrideValue;

        //생성자
        public StatOverride(StatSO stat) => this.stat = stat;

        public StatSO CreateStat()
        {
            StatSO newStat = stat.Clone() as StatSO;
            Debug.Assert(newStat != null, "StatSO clone failed. Check ICloneable correctly.");

            if (isUseOverride)
            {
                newStat.BaseValue = overrideValue;
            }
            return newStat;
        }
    }
}