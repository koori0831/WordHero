using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Work.Shops.Code
{
    [Serializable]
    class ItemData
    {
        public ShopItemDataSO item;
        [HideInInspector] public bool isShowItem;
        [HideInInspector] public bool isSoldItem;
    }

    public class Shop : MonoBehaviour
    {
        [SerializeField]
        private List<ShopTable> tableList = new List<ShopTable>();
        [SerializeField]
        private List<ItemData> itemList = new List<ItemData>();

        public void Awake()
        {
            SetTables();
        }

        private void Update()
        {
            if(Keyboard.current.kKey.wasPressedThisFrame)
                SetTables();
        }

        public void SetTables()
        {
            ResetTables();

            tableList.ForEach(x =>
            {
                List<ItemData> list = itemList.Where(x => !x.isShowItem && !x.isSoldItem).ToList();
                if (list.Count <= 0) return;
                ItemData selectItem = list[Random.Range(0, list.Count)];

                if (selectItem == null) return;

                x.SetItemData(selectItem.item);
                selectItem.isShowItem = true;
            });
        }

        public void ResetTables()
        {
            List<ItemData> list = itemList.Where(x => x.isShowItem).ToList();
            list.ForEach(x => x.isShowItem = false);

            tableList.ForEach(x =>
            {
                x.ResetTable();    
            });
        }
    }
}