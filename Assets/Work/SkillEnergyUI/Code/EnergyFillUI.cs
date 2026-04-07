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
        private int _barRefreshVersion;
        private int _barShakeVersion;
        private int _barPulseVersion;

        private List<EnergyBarUI> _currentBarUIs = new List<EnergyBarUI>();
        private List<Vector2> _defaultFillAnchoredPositions = new List<Vector2>();
        private List<Vector3> _defaultFillScales = new List<Vector3>();

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
                _defaultFillAnchoredPositions.Clear();
                _defaultFillScales.Clear();
            }

            for (int i = 0; i < _maxValue; i++)
            {
                if (i >= _currentBarUIs.Count)
                {
                    EnergyBarUI newBar = Instantiate(fillObjectPrefab, parent);
                    _currentBarUIs.Add(newBar);
                    _defaultFillAnchoredPositions.Add(newBar.FillImage.rectTransform.anchoredPosition);
                    _defaultFillScales.Add(newBar.FillImage.rectTransform.localScale);
                }
            }

            BarRefresh(current);
        }

        public async void BarRefresh(float current, float duration = 0.1f)
        {
            if (_currentBarUIs.Count == 0)
            {
                return;
            }

            int refreshVersion = ++_barRefreshVersion;
            float previousValue = GetDisplayedValue();
            bool isDecreasing = current < previousValue;

            int start = isDecreasing ? _currentBarUIs.Count - 1 : 0;
            int end = isDecreasing ? -1 : _currentBarUIs.Count;
            int step = isDecreasing ? -1 : 1;
            int pulseVersion = ++_barPulseVersion;

            for (int i = start; i != end; i += step)
            {
                float targetFill = GetTargetFill(i, current);

                if (isDecreasing)
                {
                    ApplyDecreaseImpact(i, targetFill, pulseVersion);
                }
                else
                {
                    await AnimateBarFill(i, targetFill, duration, refreshVersion, pulseVersion);
                }

                if (refreshVersion != _barRefreshVersion)
                {
                    return;
                }
            }
        }

        private float GetDisplayedValue()
        {
            float displayedValue = 0f;

            for (int i = 0; i < _currentBarUIs.Count; i++)
            {
                displayedValue += _currentBarUIs[i].FillImage.fillAmount;
            }

            return displayedValue;
        }

        private static float GetTargetFill(int index, float current)
        {
            return Mathf.Clamp01(current - index);
        }

        private async Awaitable AnimateBarFill(int index, float targetFill, float duration, int refreshVersion, int pulseVersion)
        {
            var fillImage = _currentBarUIs[index].FillImage;
            float startFill = fillImage.fillAmount;

            if (Mathf.Approximately(startFill, targetFill))
            {
                return;
            }

            if (duration <= 0f)
            {
                PlayFillPulse(index, false, pulseVersion);
                fillImage.fillAmount = targetFill;
                return;
            }

            PlayFillPulse(index, false, pulseVersion);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (refreshVersion != _barRefreshVersion)
                {
                    return;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                fillImage.fillAmount = Mathf.Lerp(startFill, targetFill, t);

                await Awaitable.NextFrameAsync();
            }

            fillImage.fillAmount = targetFill;
        }

        private void ApplyDecreaseImpact(int index, float targetFill, int pulseVersion)
        {
            var fillImage = _currentBarUIs[index].FillImage;
            float startFill = fillImage.fillAmount;

            if (Mathf.Approximately(startFill, targetFill))
            {
                return;
            }

            PlayFillPulse(index, true, pulseVersion);
            fillImage.fillAmount = targetFill;
        }

        private async void PlayFillPulse(int index, bool isDecreasing, int pulseVersion)
        {
            if (index < 0 || index >= _currentBarUIs.Count)
            {
                return;
            }

            RectTransform fillRect = _currentBarUIs[index].FillImage.rectTransform;
            Vector3 baseScale = _defaultFillScales[index];
            float peakScale = isDecreasing ? 1.22f : 1.08f;
            float peakDuration = isDecreasing ? 0.03f : 0.05f;
            float settleDuration = isDecreasing ? 0.06f : 0.08f;

            float elapsed = 0f;
            while (elapsed < peakDuration)
            {
                if (pulseVersion != _barPulseVersion)
                {
                    fillRect.localScale = baseScale;
                    return;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / peakDuration);
                float scale = Mathf.Lerp(1f, peakScale, t);
                fillRect.localScale = baseScale * scale;

                await Awaitable.NextFrameAsync();
            }

            elapsed = 0f;
            while (elapsed < settleDuration)
            {
                if (pulseVersion != _barPulseVersion)
                {
                    fillRect.localScale = baseScale;
                    return;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / settleDuration);
                float eased = 1f - ((1f - t) * (1f - t));
                float scale = Mathf.Lerp(peakScale, 1f, eased);
                fillRect.localScale = baseScale * scale;

                await Awaitable.NextFrameAsync();
            }

            fillRect.localScale = baseScale;
        }

        public async void PlayInsufficientShake(int requiredCost, float currentEnergy, float duration = 0.12f, float strength = 4f)
        {
            if (_currentBarUIs.Count == 0)
            {
                return;
            }

            int start = Mathf.Clamp(Mathf.FloorToInt(currentEnergy), 0, _currentBarUIs.Count);
            int end = Mathf.Clamp(requiredCost, 0, _currentBarUIs.Count);

            if (start >= end)
            {
                return;
            }

            int shakeVersion = ++_barShakeVersion;

            if (duration <= 0f || Mathf.Approximately(strength, 0f))
            {
                RestoreShakeTargets(start, end);
                return;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (shakeVersion != _barShakeVersion)
                {
                    return;
                }

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float damping = 1f - t;
                float offsetX = Mathf.Sin(t * Mathf.PI * 8f) * strength * damping;

                for (int i = start; i < end; i++)
                {
                    RectTransform fillRect = _currentBarUIs[i].FillImage.rectTransform;
                    Vector2 basePos = _defaultFillAnchoredPositions[i];
                    fillRect.anchoredPosition = new Vector2(basePos.x + offsetX, basePos.y);
                }

                await Awaitable.NextFrameAsync();
            }

            if (shakeVersion != _barShakeVersion)
            {
                return;
            }

            RestoreShakeTargets(start, end);
        }

        private void RestoreShakeTargets(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                RectTransform fillRect = _currentBarUIs[i].FillImage.rectTransform;
                fillRect.anchoredPosition = _defaultFillAnchoredPositions[i];
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

    }
}
