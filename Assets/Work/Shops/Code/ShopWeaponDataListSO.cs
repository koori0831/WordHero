using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.Shops.Code
{
    [System.Serializable]
    public class WeaponData
    {
        [field: SerializeField] public BaseWeapon Weapon { get; private set; }
        [field: SerializeField] public GameObject Model { get; private set; }
    }

    [CreateAssetMenu(fileName = "WeaponDataList", menuName = "SO/ItemDataList/WeaponDataListSO", order = 1)]
    public class ShopWeaponDataListSO : ScriptableObject
    {
        [field:SerializeField] public List<WeaponData> weaponDataList { get; private set; }
    }
}