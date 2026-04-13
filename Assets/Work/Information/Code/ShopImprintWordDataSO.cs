using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;

namespace Work.Information.Code
{
    [CreateAssetMenu(fileName = "ShopWeaponDataSO", menuName = "SO/Information/ShopImprintWordDataSO")]
    public class ShopImprintWordDataSO : ModelInfoDataSO
    {
        [field: SerializeField] public ImprintWordSO ImprintWord { get; private set; }

        public ShopImprintWordDataSO(ImprintWordSO imprintWord) :base()
        {
            ImprintWord = imprintWord;
        }

        public new ShopImprintWordDataSO GetInfo()
        {
            ShopImprintWordDataSO data = new ShopImprintWordDataSO(ImprintWord);
            return data;
        }
    }
}
