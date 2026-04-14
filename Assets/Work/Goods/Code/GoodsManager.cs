using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Goods.Code
{
    public record struct OnGetGoldEvent : IEvent;
    public record struct FloatReturnValue(float Value) : IReturnValue;

    public class GoodsManager : MonoBehaviour
    {
        #region Gold Region



        [field :SerializeField] public int Gold { get; private set; }

        private void Awake()
        {
            Bus<TryDecreaseGoldEvent, BooleanReturnValue>.Events += HandleTryDecreaseGoldEvent;
            Bus<OnGetGoldEvent, FloatReturnValue>.Events += HandleOnGetGoldEvent;
        }

        private BooleanReturnValue HandleTryDecreaseGoldEvent(TryDecreaseGoldEvent evt)
        {
            bool isSuccess = TryDecreaseCoin(evt.Amount);
            BooleanReturnValue value = new BooleanReturnValue(isSuccess);
            return value;
        }

        private FloatReturnValue HandleOnGetGoldEvent(OnGetGoldEvent evt)
        {
            return new FloatReturnValue(Gold);
        }

        public void SetGold(int amount) => Gold = amount;

        public void AddCoin(int amount) => Gold += amount;

        public void DecreaseCoin(int amount) => Gold -= amount;

        public int GetGold() => Gold;

        public bool TryDecreaseCoin(int amount)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                Bus<OnGoodsUIEvent>.Raise(new OnGoodsUIEvent(true, Gold));
            }
            else
                return false;
            return true;
        }


        #endregion
    }
}