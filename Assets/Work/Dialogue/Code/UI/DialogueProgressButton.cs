using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    [RequireComponent(typeof(Button))]
    public class DialogueProgressButton : MonoBehaviour
    {
        private Button _progressButton;

        private void Awake()
        {
            _progressButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _progressButton.onClick.AddListener(ProgressDialogue);
        }

        private void OnDisable()
        {
            _progressButton.onClick.RemoveListener(ProgressDialogue);
        }

        private void ProgressDialogue()
        {
            Bus<UIContinueButtonPressedEvent>.Raise(new UIContinueButtonPressedEvent());
        }
    }
}
