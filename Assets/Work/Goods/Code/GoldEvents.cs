using Work.Core.Utils.EventBus;

namespace Work.Goods.Code
{
    public class GoldEvents
    {
    }

    public struct TryDecreaseGoldEvent : IEvent
    {
        public int Amount { get; private set; }

        public TryDecreaseGoldEvent(int amount) { Amount = amount; }
    }

    public struct BooleanReturnValue : IReturnValue
    {
        public bool Value { get; private set; }

        public BooleanReturnValue(bool value) { Value = value; }
    }
}
