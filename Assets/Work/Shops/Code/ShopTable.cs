using System;
using UnityEngine;
using Work.Information.Code;

namespace Work.Shops.Code
{
    public class ShopTable : MonoBehaviour
    {
        private ShopItemDataSO _currentItem;
        private GameObject _model;
        [SerializeField] private Transform modelTrm;


        public void SetItemData(ShopItemDataSO data)
        {
            _currentItem = data;

            if (_model != null)
                Destroy(_model);
            _model = Instantiate(_currentItem.ModelInfo.Model,modelTrm);
        }

        public void ResetTable() // 아이템을 구매하였을떄 
        {
            if(_model != null)
                Destroy(_model);
            if(_currentItem)
                _currentItem = null;
        }
    }
}