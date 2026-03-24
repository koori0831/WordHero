using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Goods.Code
{
    public class GoodsManager : MonoBehaviour
    {
        #region Gold Region



        [field :SerializeField] public int Gold { get; private set; }

        private void Awake()
        {
            Bus<TryDecreaseGoldEvent, BooleanReturnValue>.Events += HandleTryDecreaseGoldEvent;
        }

        private BooleanReturnValue HandleTryDecreaseGoldEvent(TryDecreaseGoldEvent evt)
        {
            bool isSuccess = TryDecreaseCoin(evt.Amount);
            BooleanReturnValue value = new BooleanReturnValue(isSuccess);
            return value;
        }

        public void SetGold(int amount) => Gold = amount;

        public void AddCoin(int amount) => Gold += amount;

        public void DecreaseCoin(int amount) => Gold -= amount;

        public bool TryDecreaseCoin(int amount)
        {
            if (Gold >= amount)
                Gold -= amount;
            else
                return false;
            return true;
        }


        #endregion
    }
}