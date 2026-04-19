using LitMotion;
using TMPro;
using UnityEngine;

namespace Work.Combat.Code
{
    public class DamageText : MonoBehaviour
    {
        private bool _isCritical;
        private float _damage;

        [SerializeField] private TMPro.TextMeshPro _textMeshPro;
        [SerializeField] private TMP_FontAsset criticalFontAsset, playerFontAsset;


        public void Init(int damage, bool isCritical = false, bool isPlayer = false)
        {
            _damage = damage;
            _isCritical = isCritical;

            if (!isPlayer)
                _textMeshPro.color = _isCritical ? Color.yellow : Color.white;
            else
                _textMeshPro.color = Color.red;


            _textMeshPro.text = _damage.ToString();
            if (isCritical)
                _textMeshPro.font = criticalFontAsset;

            if (isPlayer)
                _textMeshPro.font = playerFontAsset;

                float maxFontSize = 0;
            float minFontSize = 0;

            float startFontMaxSize = _isCritical ? 19f : 13f;
            float startFontMinSize = _isCritical ? 18f : 12f;

            float endFontMaxSize = _isCritical ? 8f : 5f;
            float endFontMinSize = _isCritical ? 7f : 4f;

            maxFontSize = Random.Range(startFontMinSize, startFontMaxSize);
            minFontSize = Random.Range(endFontMinSize, endFontMaxSize);

            _textMeshPro.fontSize = minFontSize;

            LMotion.Create(minFontSize, maxFontSize, 0.06f).WithOnComplete(() =>
            {
                LMotion.Create(maxFontSize, minFontSize, 0.07f).WithOnComplete(() =>
                {
                    LMotion.Create(minFontSize, minFontSize + 0.2f, 1.2f).WithOnComplete(() =>
                    {
                        Destroy(gameObject);
                    }).Bind(x => _textMeshPro.fontSize = x);
                }).Bind(x => _textMeshPro.fontSize = x);
            }).Bind(x => _textMeshPro.fontSize = x);
        }
    }
}