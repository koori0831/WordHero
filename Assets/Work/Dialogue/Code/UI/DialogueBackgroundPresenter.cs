using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    public class DialogueBackgroundPresenter : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;

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
            SetBackground(evt.BackgroundSprite);
        }

        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            SetBackground(null);
        }

        private void SetBackground(Sprite sprite)
        {
            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = sprite;
            backgroundImage.enabled = sprite != null;
        }
    }
}
