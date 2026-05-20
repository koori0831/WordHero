using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    [RequireComponent(typeof(Button))]
    public class DialogueChoiceButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI choiceText;

        private Button _button;
        private DialogueChoiceViewData _choiceData;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

        public void Setup(DialogueChoiceViewData choice)
        {
            _choiceData = choice;

            if (choiceText != null)
            {
                choiceText.text = choice.ChoiceText;
            }
        }

        private void OnClick()
        {
            Bus<DialogueChoiceSelectedEvent>.Raise(new DialogueChoiceSelectedEvent(_choiceData.ChoiceIndex));
        }
    }
}
