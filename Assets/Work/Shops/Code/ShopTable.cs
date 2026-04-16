using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Goods.Code;
using Work.Information.Code;
using Work.Players.Code;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;

namespace Work.Shops.Code
{
    public class ShopTable : MonoBehaviour, IInteractable
    {
        private ShopItemData _currentItem;
        private GameObject _model;
        [SerializeField] private Transform modelTrm;
        [SerializeField] private Vector3 size;
        [SerializeField] private ShopItemDescUI shopItemDescUI;


        public bool IsTableBuy { get; private set; } = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_currentItem == null) return;
            if (other.gameObject.tag == "Player")
            {
                shopItemDescUI.HandleShowItemInfo(_currentItem.item.Name, _currentItem.Price);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                shopItemDescUI.HandleHideItemInfo();
            }
        }

        public void SetItemData(ShopItemData data)
        {
            if (IsTableBuy) return;
            _currentItem = data;

            if (_model != null)
                Destroy(_model);
            _model = Instantiate(_currentItem.model, modelTrm);
        }

        public void ResetTable() // 아이템을 구매하였을떄 
        {
            shopItemDescUI.HandleHideItemInfo();
            if (_model != null)
                Destroy(_model);
            if (_currentItem != null)
            {
                _currentItem = null;
            }
        }

        public void Interact(GameObject interactor)
        {
            bool value = Bus<TryDecreaseGoldEvent, BooleanReturnValue>.Raise(new TryDecreaseGoldEvent(_currentItem.Price)).Value;
            if (value)
            {
                Player player = interactor.GetComponent<Player>();

                if (_currentItem is ShopWeaponItemData weaponItemData)
                    player.GetWeapon(Instantiate(weaponItemData.Weapon));
                if(_currentItem is ShopWordItemData wordItemData)
                    player.GetImprintWord(wordItemData.item as ImprintWordSO,1);

                IsTableBuy = true;
                ResetTable();
            }
        }
    }
}
