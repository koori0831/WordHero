using LitMotion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.AcquireItem.Code
{
    public class AcquireItemFieldSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private Image colorImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float lifeTime = 1;

        private float _timer;

        public void Init()
        {
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0);
            typeText.color = new Color(typeText.color.r, typeText.color.g, typeText.color.b, 0);
            colorImage.color = new Color(colorImage.color.r, colorImage.color.g, colorImage.color.b, 0);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0);
            _timer = lifeTime;
            SetAlpha(true,0.3f);
        }

        public void SetAlpha(bool inActiveTrue, float duration)
        {
            float start = inActiveTrue ? 0f : 1f;
            float end = inActiveTrue ? 1f : 0f;

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

        public void SetInfo(string name, string type, Color color)
        {
            nameText.text = name;
            typeText.text = type;
            colorImage.color = color;
        }
    }
}