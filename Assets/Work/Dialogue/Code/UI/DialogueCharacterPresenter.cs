using System;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    public class DialogueCharacterPresenter : MonoBehaviour
    {
        [Serializable]
        public class CharacterSlot
        {
            public GameObject Root;
            public Image Image;
            public CanvasGroup Group;
        }

        [SerializeField] private CharacterSlot leftSlot;
        [SerializeField] private CharacterSlot rightSlot;
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private void Awake()
        {
            HideSlot(leftSlot);
            HideSlot(rightSlot);
        }

        private void OnEnable()
        {
            Bus<DialogueProgressEvent>.Events += OnDialogueProgress;
            Bus<DialogueEndEvent>.Events += OnDialogueEnd;
        }

        private void OnDisable()
        {
            Bus<DialogueProgressEvent>.Events -= OnDialogueProgress;
            Bus<DialogueEndEvent>.Events -= OnDialogueEnd;
        }

        private void OnDialogueProgress(DialogueProgressEvent evt)
        {
            if (evt.CharacterSprite == null)
            {
                HideSlot(leftSlot);
                HideSlot(rightSlot);
                return;
            }

            bool useLeftSlot = evt.NameTagPosition == NameTagPositionType.Left;
            CharacterSlot activeSlot = useLeftSlot ? leftSlot : rightSlot;
            CharacterSlot inactiveSlot = useLeftSlot ? rightSlot : leftSlot;

            ShowSlot(activeSlot, evt.CharacterSprite, activeColor);
            DimSlot(inactiveSlot);
        }

        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            HideSlot(leftSlot);
            HideSlot(rightSlot);
        }

        private void ShowSlot(CharacterSlot slot, Sprite sprite, Color color)
        {
            if (slot == null)
            {
                return;
            }

            if (slot.Root != null)
            {
                slot.Root.SetActive(true);
            }

            if (slot.Group != null)
            {
                slot.Group.alpha = 1f;
            }

            if (slot.Image != null)
            {
                slot.Image.sprite = sprite;
                slot.Image.color = color;
                slot.Image.preserveAspect = true;
                slot.Image.enabled = true;
            }
        }

        private void DimSlot(CharacterSlot slot)
        {
            if (slot?.Image == null || slot.Image.sprite == null)
            {
                return;
            }

            if (slot.Root != null)
            {
                slot.Root.SetActive(true);
            }

            if (slot.Group != null)
            {
                slot.Group.alpha = 1f;
            }

            slot.Image.color = inactiveColor;
        }

        private void HideSlot(CharacterSlot slot)
        {
            if (slot == null)
            {
                return;
            }

            if (slot.Image != null)
            {
                slot.Image.sprite = null;
                slot.Image.enabled = false;
            }

            if (slot.Group != null)
            {
                slot.Group.alpha = 0f;
            }

            if (slot.Root != null)
            {
                slot.Root.SetActive(false);
            }
        }
    }
}
