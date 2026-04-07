using UnityEngine;
using Work.Weapons.Code;
using System.Collections.Generic;

namespace Work.Weapons.Imprint.Code
{
    [CreateAssetMenu(fileName = "ImprintWordSO", menuName = "SO/ImprintWord", order = 1)]
    public class ImprintWordSO : ScriptableObject
    {
        [Header("Info")]
        public string DisplayName;
        public ImprintType Type;
        public Sprite Icon;
        [TextArea] public string Description;

        [SerializeReference]
        public List<ISkillEffect> Effects;
    }
}
