using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code
{
    [CreateAssetMenu(fileName = "ImprintWordListSO", menuName = "SO/ImprintWordList", order = 2)]
    public class ImprintWordListSO : ScriptableObject
    {
        [field:SerializeField] public List<ImprintWordSO> Words { get; private set; }
    }
}
