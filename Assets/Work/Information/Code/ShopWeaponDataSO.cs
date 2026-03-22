using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Information.Code
{
    [CreateAssetMenu(fileName = "ShopWeaponDataSO", menuName = "SO/Information/ShopWeaponDataSO")]
    public class ShopWeaponDataSO : ModelInfoDataSO
    {
        [field: SerializeField] public BaseWeapon BaseWeapon {  get; private set; }

        public new ShopWeaponDataSO GetInfo()
        {
            ShopWeaponDataSO data = new ShopWeaponDataSO();
            return data;
        }
    }
}
