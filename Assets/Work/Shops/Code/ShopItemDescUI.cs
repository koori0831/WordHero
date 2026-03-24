using TMPro;
using UnityEngine;

namespace Work.Shops.Code
{
    public class ShopItemDescUI : MonoBehaviour
    {
        [SerializeField] private Transform background;
        [SerializeField] private TextMeshPro nameField;
        [SerializeField] private TextMeshPro goldField;

        public bool IsShow { get; private set; }

        private void Awake()
        {
            HandleHideItemInfo();
        }

        public void HandleHideItemInfo()
        {
            IsShow = false;
            background.gameObject.SetActive(false);
        }

        public void HandleShowItemInfo(string name, int price)
        {
            IsShow = true;
            background.gameObject.SetActive(true);
            nameField.text = name;
            goldField.text = price.ToString() + " <color=yellow>Gold</color>";
        }
    }
}