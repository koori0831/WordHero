using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Information.Code;
using Work.Weapons.Code;
using Work.Weapons.Imprint.Code;
using Random = UnityEngine.Random;

namespace Work.Shops.Code
{
    [Serializable]
    public class ShopItemData
    {
        public InfoDataSO item;
        public GameObject model;
        public int Price;
    }

    [Serializable]
    public class ShopWordItemData : ShopItemData
    {
        
    }

    [Serializable]
    public class ShopWeaponItemData : ShopItemData
    {
        public BaseWeapon Weapon { get; set; } 
    }

    public class Shop : MonoBehaviour
    {
        [SerializeField]
        private List<ShopTable> tableList = new List<ShopTable>();
        private List<ShopItemData> itemList = new List<ShopItemData>();
        [SerializeField] private ImprintWordListSO wordListSO;
        [SerializeField] private List<WordModel> wordModels;
        [SerializeField] private ShopWeaponDataListSO weaponListSO;

        public void Awake()
        {
            foreach (var word in wordListSO.Words)
            {
                ShopWordItemData itemData = new ShopWordItemData();
                itemData.item = word;
                itemData.model = wordModels.Find(x => x.WordType == word.Type)?.gameObject;

                switch(word.Type)
                {
                    case ImprintType.Attack:
                        itemData.Price = 50;
                        break;
                    case ImprintType.Effect:
                        itemData.Price = 35;
                        break;
                    case ImprintType.Stat:
                        itemData.Price = 35;
                        break;
                }

                itemList.Add(itemData);
            }

            foreach (var weapon in weaponListSO.weaponDataList)
            {
                ShopWeaponItemData itemData = new ShopWeaponItemData();
                itemData.item = weapon.Weapon.Data;
                itemData.model = weapon.Model;
                itemData.Weapon = weapon.Weapon;
                itemData.Price = 40;
                itemList.Add(itemData);
            }

            SetTables();
        }

        public void SetTables()
        {
            ResetTables();

            tableList.ForEach(x =>
            {
                if (!x.IsTableBuy)
                {
                    if (itemList.Count <= 0) return;
                    ShopItemData selectItem = itemList[Random.Range(0, itemList.Count)];

                    if (selectItem == null) return;

                    x.SetItemData(selectItem);
                }
            });
        }

        public void ResetTables()
        {

            tableList.ForEach(x =>
            {
                x.ResetTable();
            });
        }
    }
}