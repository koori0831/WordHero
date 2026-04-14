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
        [field: SerializeField] public WeaponDataSO WeaponDataSO { get; private set; }

        private void OnValidate()
        {
            if(BaseWeapon != null)
            {
                Name = WeaponDataSO.WeaponName;
                Description = WeaponDataSO.WeaponDescription;
            }
        }

        public new ShopWeaponDataSO GetInfo()
        {
            ShopWeaponDataSO data = new ShopWeaponDataSO();
            data.BaseWeapon = BaseWeapon;
            data.WeaponDataSO = WeaponDataSO;
            data.Name = Name;
            data.Description = Description;
            return data;
        }
    }
}
