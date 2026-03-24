using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Goods.Code;
using Work.Information.Code;
using Work.Players.Code;

namespace Work.Shops.Code
{
    public class ShopTable : MonoBehaviour, IInteractable
    {
        private ItemData _currentItem;
        private GameObject _model;
        [SerializeField] private Transform modelTrm;
        [SerializeField] private Vector3 size;
        [SerializeField] private ShopItemDescUI shopItemDescUI;

        private bool _isInBoundry;

        private void OnTriggerEnter(Collider other)
        {
            if (_currentItem == null) return;
            if (other.gameObject.tag == "Player")
            {
                shopItemDescUI.HandleShowItemInfo(_currentItem.item.ModelInfo.Name, _currentItem.item.Price);
                _isInBoundry = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                shopItemDescUI.HandleHideItemInfo();
                _isInBoundry = false;
            }
        }

        public void SetItemData(ItemData data)
        {
            _currentItem = data;

            if (_model != null)
                Destroy(_model);
            _model = Instantiate(_currentItem.item.ModelInfo.Model, modelTrm);
        }

        public void ResetTable() // 아이템을 구매하였을떄 
        {
            shopItemDescUI.HandleHideItemInfo();
            if (_model != null)
                Destroy(_model);
            if (_currentItem != null)
            {
                _currentItem.isShowItem = true;
                _currentItem = null;
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!_isInBoundry) return;
            if (_currentItem == null) return;
            _currentItem.isSoldItem = true;
            BooleanReturnValue value = Bus<TryDecreaseGoldEvent, BooleanReturnValue>.Raise(new TryDecreaseGoldEvent(_currentItem.item.Price));
            if (value.Value)
            {
                Player player = interactor.GetComponent<Player>();

                if (_currentItem.item.ModelInfo is ShopWeaponDataSO data)
                    player.GetWeapon(Instantiate(data.BaseWeapon));


                ResetTable();
            }
        }
    }
}