using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Goods.Code;
using Work.Shops.Code;

namespace Work.NPC.Code
{
    public class ShopNPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private Shop shop;
        private int _resetCoin = 50;

        public void Interact(GameObject interactor)
        {
            bool isTrue = Bus<TryDecreaseGoldEvent, BooleanReturnValue>.Raise(new TryDecreaseGoldEvent(_resetCoin)).Value;
            if (isTrue)
                shop.ResetTables();
        }
    }
}