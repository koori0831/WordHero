using System.Collections.Generic;
using UnityEngine;
using Work.HPBar.Code;

namespace Work.SkillEnergyUI.Code
{

    public class EnergyFillUI : MonoBehaviour, IUIElement<float, int>
    {
        [SerializeField] private EnergyBarUI fillObjectPrefab;
        [SerializeField] private RectTransform parent;

        private int _maxValue;

        private List<EnergyBarUI> _currentBarUIs = new List<EnergyBarUI>();

        public void EnableFor(float currentValue, int maxValue)
        {
            gameObject.SetActive(true);

            if (_maxValue != maxValue)
            {
                _maxValue = maxValue;
            }

            RefreshUI(currentValue);
        }

        public void RefreshUI(float current)
        {
            if (_maxValue < _currentBarUIs.Count)
            {
                _currentBarUIs.Clear();
            }

            for (int i = 0; i < _maxValue; i++)
            {
                if (i >= _currentBarUIs.Count)
                {
                    EnergyBarUI newBar = Instantiate(fillObjectPrefab, parent);
                    _currentBarUIs.Add(newBar);
                }
            }

            BarRefresh(current);
        }

        public async void BarRefresh(float current, float time = 0.5f)
        {
            float currentValue = current;

            for (int i = 0; i < _currentBarUIs.Count; i++)
            {
                float timer = 0f;
                // 1 : 0.5f = current - i : newtime
                float newtime = time * Mathf.Abs(current - i);
                float value = 0;

                if (currentValue - 1f >= 1f)
                {
                    value = 1;
                    currentValue -= 1f;
                }
                else
                {
                    value = currentValue;
                }

                while (newtime >= timer)
                {
                    value = value * (1 - timer / newtime);
                    _currentBarUIs[i].FillImage.fillAmount = value;
                    await Awaitable.FixedUpdateAsync();
                }
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

    }
}