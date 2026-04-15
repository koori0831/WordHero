using System.Collections.Generic;
using UnityEngine;
using Work.Information.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code
{
    [CreateAssetMenu(fileName = "ImprintWordSO", menuName = "SO/ImprintWord", order = 1)]
    public class ImprintWordSO : InfoDataSO
    {
        [Header("Info")]
        public ImprintType Type;
        public Sprite Icon;

        [SerializeReference]
        public List<ISkillEffect> Effects;

    }
}
