using TMPro;
using UnityEngine;

namespace Work.Sentence.Code.UI
{
    public class SentenceSettingView : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private TMP_Text partName, coreWord, wordA, wordB;

        public void SetPartName(string name)
        {
            if (partName != null)
            {
                partName.text = name;
            }
        }

        public void SetCoreWord(string word)
        {
            if (coreWord != null)
            {
                coreWord.text = word;
            }
        }

        public void SetWordA(string word)
        {
            if (wordA != null)
            {
                wordA.text = word;
            }
        }

        public void SetWordB(string word)
        {
            if (wordB != null)
            {
                wordB.text = word;
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(isVisible);
            }
            else
            {
                gameObject.SetActive(isVisible);
            }

            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = isVisible ? 1f : 0f;
                panelCanvasGroup.interactable = isVisible;
                panelCanvasGroup.blocksRaycasts = isVisible;
            }
        }
    }
}
