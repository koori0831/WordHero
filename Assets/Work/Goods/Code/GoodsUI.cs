using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.RequestInjectors;
using Work.Core.Utils.EventBus;
using Work.Players.Code;

namespace Work.Goods.Code
{
    public record struct OnGoodsUIEvent(bool isShow, int coinAmount = -1) : IEvent;

    public class GoodsUI : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private Image barImage;
        [SerializeField] private TextMeshProUGUI coinText;

        public void Awake()
        {
            Bus<OnGoodsUIEvent>.Events += HandleGoodsUIEvent;
        }

        private void OnDestroy()
        {
            Bus<OnGoodsUIEvent>.Events -= HandleGoodsUIEvent;
        }

        public void HandleGoodsUIEvent(OnGoodsUIEvent evt)
        {
            if (evt.isShow)
            {
                if (evt.coinAmount == -1)
                {
                    FloatReturnValue returnValue = Bus<OnGetGoldEvent, FloatReturnValue>.Raise(new OnGetGoldEvent());
                    coinText.text = returnValue.Value.ToString();
                }
                else
                    coinText.text = evt.coinAmount.ToString();

                root.gameObject.SetActive(true);
                barImage.rectTransform.sizeDelta.Set(coinText.rectTransform.sizeDelta.x + 100f, 1);
            }
            else
            {
                root.gameObject.SetActive(false);
            }
        }
    }
}