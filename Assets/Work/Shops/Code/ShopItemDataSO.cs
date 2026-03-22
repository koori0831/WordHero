using UnityEngine;
using Work.Information.Code;

namespace Work.Shops.Code
{
    public enum ShopItemType
    {
        None,
        Text,
        Weapon,
    }

    [CreateAssetMenu(fileName = "ShopItemDataSO", menuName = "SO/Shops/ShopItemDataSO")]
    public class ShopItemDataSO : ScriptableObject
    {
        [SerializeField] private ShopItemType type;
        [field:SerializeField] public int Price {  get; private set; }
        [field:SerializeField] public ModelInfoDataSO ModelInfo {  get; private set; }

        public bool _isText => type == ShopItemType.Text;
        public bool _isWeapon => type == ShopItemType.Weapon;
    }
}