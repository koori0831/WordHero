using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.Test
{
    public class DialogueTester : MonoBehaviour
    {
        [SerializeField] private DialogueInformationSO dialogueInformationSO;
        [SerializeField] private bool playOnStart = true;

        private void Start()
        {
            if (playOnStart)
            {
                Play();
            }
        }

        public void Play()
        {
            if (dialogueInformationSO == null)
            {
                return;
            }

            Bus<DialogueStartEvent>.Raise(new DialogueStartEvent(dialogueInformationSO));
        }
    }
}
