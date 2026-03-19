using UnityEngine;

namespace Work.Shops.Code
{
    public enum ShopItemType
    {
        None,
        Text,
        Weapon,
    }

    [CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "Scriptable Objects/ShopItemDataSO")]
    public class ShopItemDataSO : ScriptableObject
    {
        [SerializeField] private ShopItemType type;

        private bool _isText => type == ShopItemType.Text;
        private bool _isWeapon => type == ShopItemType.Weapon;


    }
}