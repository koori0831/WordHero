using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Work.Sentence.Code.UI
{
    public class SentenceSettingView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TMP_Text partName, coreWord, wordA, wordB;
        [SerializeField] private Button previousPartButton;
        [SerializeField] private Button nextPartButton;
        [SerializeField] private Transform inventoryRoot;
        [SerializeField] private WordItemUIElement wordItemPrefab;
        [SerializeField] private RectTransform[] layoutRefreshTargets;

        private readonly List<WordItemUIElement> _wordItems = new List<WordItemUIElement>(16);
        private bool _layoutRefreshQueued;

        private void Awake()
        {
            if (inventoryRoot == null)
            {
                RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);
                for (int i = 0; i < rects.Length; i++)
                {
                    if (rects[i].name == "Content")
                    {
                        inventoryRoot = rects[i];
                        break;
                    }
                }
            }

            if (inventoryRoot == null)
            {
                inventoryRoot = transform;
            }

            if (wordItemPrefab == null)
            {
                wordItemPrefab = inventoryRoot.GetComponentInChildren<WordItemUIElement>(true);
            }

            if (wordItemPrefab == null && inventoryRoot.childCount > 0)
            {
                Transform template = inventoryRoot.Find("WordItem");
                if (template == null)
                {
                    template = inventoryRoot.GetChild(0);
                }

                GameObject templateObject = template.gameObject;
                wordItemPrefab = templateObject.GetComponent<WordItemUIElement>();
                if (wordItemPrefab == null)
                {
                    wordItemPrefab = templateObject.AddComponent<WordItemUIElement>();
                }
            }

            if (wordItemPrefab != null)
            {
                wordItemPrefab.gameObject.SetActive(false);
            }

            ResolveLayoutTargetsIfNeeded();
        }

        public void SetPartName(string name)
        {
            if (partName != null)
            {
                partName.text = name;
            }
        }

        public void BindPartSelection(Action onPrevious, Action onNext)
        {
            if (previousPartButton != null)
            {
                previousPartButton.onClick.RemoveAllListeners();
                previousPartButton.onClick.AddListener(() => onPrevious?.Invoke());
            }

            if (nextPartButton != null)
            {
                nextPartButton.onClick.RemoveAllListeners();
                nextPartButton.onClick.AddListener(() => onNext?.Invoke());
            }
        }

        public void SetPartNavigationEnabled(bool enabled)
        {
            if (previousPartButton != null)
            {
                previousPartButton.gameObject.SetActive(enabled);
                previousPartButton.interactable = enabled;
            }

            if (nextPartButton != null)
            {
                nextPartButton.gameObject.SetActive(enabled);
                nextPartButton.interactable = enabled;
            }
        }

        public void SetCoreWord(string word)
        {
            if (coreWord != null)
            {
                coreWord.text = word;
            }

            QueueLayoutRefresh();
        }

        public void SetWordA(string word)
        {
            if (wordA != null)
            {
                wordA.text = word;
            }

            QueueLayoutRefresh();
        }

        public void SetWordB(string word)
        {
            if (wordB != null)
            {
                if (!wordB.gameObject.activeSelf)
                {
                    wordB.gameObject.SetActive(true);
                }

                wordB.text = word;
            }

            QueueLayoutRefresh();
        }

        public void SetInventoryItems(IReadOnlyList<SentenceInventoryItemSnapshot> items, Action<int> onItemButtonClicked)
        {
            if (inventoryRoot == null || wordItemPrefab == null)
            {
                return;
            }

            int count = items != null ? items.Count : 0;

            while (_wordItems.Count < count)
            {
                WordItemUIElement element = Instantiate(wordItemPrefab, inventoryRoot);
                element.gameObject.SetActive(true);
                _wordItems.Add(element);
            }

            for (int i = 0; i < _wordItems.Count; i++)
            {
                bool active = i < count;
                WordItemUIElement element = _wordItems[i];
                element.gameObject.SetActive(active);
                if (!active)
                {
                    continue;
                }

                SentenceInventoryItemSnapshot item = items[i];
                element.SetWordText(item.WordName);
                element.SetBtnText(item.IsEquipped ? "Unequip" : "Equip");

                int index = i;
                element.OnBtnClicked = () => onItemButtonClicked?.Invoke(index);
            }

            QueueLayoutRefresh();
        }

        public void SetVisible(bool isVisible)
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }
            else
            {
                gameObject.SetActive(isVisible);
            }

            if (panelCanvasGroup != null)
            {
                LMotion.Create(panelCanvasGroup.alpha, isVisible ? 1f : 0f, 0.12f)
                    .WithEase(Ease.OutQuad)
                    .WithOnComplete(() =>
                    {
                        panelCanvasGroup.interactable = isVisible;
                        panelCanvasGroup.blocksRaycasts = isVisible;
                    })
                    .BindToAlpha(panelCanvasGroup);
            }

            if (isVisible)
            {
                QueueLayoutRefresh();
            }
        }

        private void QueueLayoutRefresh()
        {
            RefreshLayoutNow();

            if (_layoutRefreshQueued)
            {
                return;
            }

            _layoutRefreshQueued = true;
            StartCoroutine(RefreshLayoutNextFrame());
        }

        private IEnumerator RefreshLayoutNextFrame()
        {
            yield return null;
            _layoutRefreshQueued = false;
            RefreshLayoutNow();
        }

        private void RefreshLayoutNow()
        {
            ResolveLayoutTargetsIfNeeded();
            Canvas.ForceUpdateCanvases();

            if (layoutRefreshTargets == null)
            {
                return;
            }

            for (int i = 0; i < layoutRefreshTargets.Length; i++)
            {
                RectTransform target = layoutRefreshTargets[i];
                if (target == null)
                {
                    continue;
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(target);
            }
        }

        private void ResolveLayoutTargetsIfNeeded()
        {
            if (layoutRefreshTargets != null && layoutRefreshTargets.Length > 0)
            {
                return;
            }

            List<RectTransform> targets = new List<RectTransform>(4);
            TryAddLayoutTarget(coreWord, targets);
            TryAddLayoutTarget(wordA, targets);
            TryAddLayoutTarget(wordB, targets);

            if (inventoryRoot is RectTransform inventoryRect && !targets.Contains(inventoryRect))
            {
                targets.Add(inventoryRect);
            }

            layoutRefreshTargets = targets.ToArray();
        }

        private static void TryAddLayoutTarget(Component source, List<RectTransform> targets)
        {
            if (source == null)
            {
                return;
            }

            LayoutGroup group = source.GetComponentInParent<LayoutGroup>(true);
            if (group == null)
            {
                return;
            }

            RectTransform rect = group.transform as RectTransform;
            if (rect == null || targets.Contains(rect))
            {
                return;
            }

            targets.Add(rect);
        }
    }
}
