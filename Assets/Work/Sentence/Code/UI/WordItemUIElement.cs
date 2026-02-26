using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.Sentence.Code.UI
{
    public class WordItemUIElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _wordText;
        [SerializeField] private TMP_Text _btnText;
        [SerializeField] private Button _btn;

        public Action OnBtnClicked;

        private UnityEngine.Events.UnityAction _cachedClickAction;

        public void SetWordText(string word)
        {
            if (_wordText != null)
            {
                _wordText.text = word;
            }
        }

        public void SetBtnText(string text)
        {
            if (_btnText != null)
            {
                _btnText.text = text;
            }
        }

        private void Awake()
        {
            ResolveReferencesIfNeeded();
            if (_btn == null)
            {
                return;
            }

            _cachedClickAction = () => OnBtnClicked?.Invoke();
            _btn.onClick.AddListener(_cachedClickAction);
        }

        private void OnDestroy()
        {
            if (_btn != null && _cachedClickAction != null)
            {
                _btn.onClick.RemoveListener(_cachedClickAction);
            }
        }

        private void ResolveReferencesIfNeeded()
        {
            if (_btn == null)
            {
                _btn = GetComponentInChildren<Button>(true);
            }

            if (_btnText == null && _btn != null)
            {
                _btnText = _btn.GetComponentInChildren<TMP_Text>(true);
            }

            if (_wordText == null)
            {
                TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    if (texts[i] == _btnText)
                    {
                        continue;
                    }

                    _wordText = texts[i];
                    break;
                }
            }
        }
    }
}
