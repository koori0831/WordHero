using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.AcquireItem.Code
{
    /// <summary>
    /// 획득 아이템 슬롯 뷰
    /// </summary>
    public class AcquireItemSlotView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private Image colorImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float lifeTime = 1;

        private float _timer;

        /// <summary>
        /// 초기 표시 상태
        /// </summary>
        public void Init()
        {
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0);
            typeText.color = new Color(typeText.color.r, typeText.color.g, typeText.color.b, 0);
            colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 0);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0);
            _timer = lifeTime;
            SetAlpha(true, 0.3f);
        }

        /// <summary>
        /// 알파 연출 처리
        /// </summary>
        public void SetAlpha(bool isFadeIn, float duration)
        {
            float start = isFadeIn ? 0f : 1f;
            float end = isFadeIn ? 1f : 0f;

            LMotion.Create(start, end, duration)
                .Bind(a =>
                {
                    nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, a);
                    typeText.color = new Color(typeText.color.r, typeText.color.g, typeText.color.b, a);
                    colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, a);
                    backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, a);
                });

        }

        private void Update()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    SetAlpha(false, 0.25f);
                    Destroy(gameObject, 0.4f);
                }
            }
        }

        /// <summary>
        /// 슬롯 정보 표시
        /// </summary>
        public void SetInfo(string itemName, string itemType, Color color)
        {
            nameText.text = itemName;
            typeText.text = itemType;
            colorImage.color = color;
        }
    }
}
