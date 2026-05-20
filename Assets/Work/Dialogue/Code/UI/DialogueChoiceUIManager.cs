using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    public class DialogueChoiceUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Transform choiceButtonContainer;

        private readonly List<GameObject> _activeButtons = new List<GameObject>();

        private void OnEnable()
        {
            Bus<DialogueShowChoiceEvent>.Events += OnShowChoice;
            Bus<DialogueChoiceSelectedEvent>.Events += OnChoiceSelected;
            Bus<DialogueEndEvent>.Events += OnDialogueEnd;
        }

        private void OnDisable()
        {
            Bus<DialogueShowChoiceEvent>.Events -= OnShowChoice;
            Bus<DialogueChoiceSelectedEvent>.Events -= OnChoiceSelected;
            Bus<DialogueEndEvent>.Events -= OnDialogueEnd;
            ClearButtons();
        }

        private void OnShowChoice(DialogueShowChoiceEvent evt)
        {
            ClearButtons();

            if (evt.Choices == null || evt.Choices.Count == 0)
            {
                SetContainerActive(false);
                return;
            }

            if (choiceButtonPrefab == null || choiceButtonContainer == null)
            {
                Debug.LogError("DialogueChoiceUIManager: choice button prefab or container is missing.");
                return;
            }

            SetContainerActive(true);

            foreach (DialogueChoiceViewData choice in evt.Choices)
            {
                GameObject buttonObject = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                if (buttonObject.TryGetComponent(out DialogueChoiceButton choiceButton))
                {
                    choiceButton.Setup(choice);
                }

                _activeButtons.Add(buttonObject);
            }
        }

        private void OnChoiceSelected(DialogueChoiceSelectedEvent evt)
        {
            ClearButtons();
            SetContainerActive(false);
        }

        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            ClearButtons();
            SetContainerActive(false);
        }

        private void SetContainerActive(bool active)
        {
            if (choiceButtonContainer != null)
            {
                choiceButtonContainer.gameObject.SetActive(active);
            }
        }

        private void ClearButtons()
        {
            for (int i = 0; i < _activeButtons.Count; i++)
            {
                if (_activeButtons[i] != null)
                {
                    Destroy(_activeButtons[i]);
                }
            }

            _activeButtons.Clear();
        }
    }
}
