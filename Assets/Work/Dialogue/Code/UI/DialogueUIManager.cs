using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    public class DialogueUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueUiParent;

        private void Awake()
        {
            if (dialogueUiParent != null)
            {
                dialogueUiParent.SetActive(false);
            }
        }

        private void OnEnable()
        {
            Bus<DialogueStartEvent>.Events += OnDialogueStart;
            Bus<DialogueEndEvent>.Events += OnDialogueEnd;
        }

        private void OnDisable()
        {
            Bus<DialogueStartEvent>.Events -= OnDialogueStart;
            Bus<DialogueEndEvent>.Events -= OnDialogueEnd;
        }

        private void OnDialogueStart(DialogueStartEvent evt)
        {
            if (dialogueUiParent != null)
            {
                dialogueUiParent.SetActive(true);
            }
        }

        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            if (dialogueUiParent != null)
            {
                dialogueUiParent.SetActive(false);
            }
        }
    }
}
