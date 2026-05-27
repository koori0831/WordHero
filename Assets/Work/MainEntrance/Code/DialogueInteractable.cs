using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Dialogue.Code;
using Work.Input.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// NPC 상호작용으로 지정된 다이얼로그를 시작하는 컴포넌트.
    /// </summary>
    public sealed class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueInformationSO dialogueInformation;
        [SerializeField] private bool lockPlayerInput = true;

        private bool _isPlaying;
        private bool _inputLocked;

        /// <summary>
        /// 다이얼로그 종료 이벤트 구독.
        /// </summary>
        private void OnEnable()
        {
            Bus<DialogueEndEvent>.Events += OnDialogueEnd;
        }

        /// <summary>
        /// 다이얼로그 종료 이벤트 구독 해제 및 입력 잠금 복구.
        /// </summary>
        private void OnDisable()
        {
            Bus<DialogueEndEvent>.Events -= OnDialogueEnd;

            if (_inputLocked)
            {
                _inputLocked = false;
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
            }

            _isPlaying = false;
        }

        /// <summary>
        /// 연결된 다이얼로그 시작.
        /// </summary>
        /// <param name="interactor">상호작용을 수행한 오브젝트.</param>
        public void Interact(GameObject interactor)
        {
            if (_isPlaying || dialogueInformation == null)
            {
                return;
            }

            _isPlaying = true;

            if (lockPlayerInput)
            {
                _inputLocked = true;
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            }

            Bus<DialogueStartEvent>.Raise(new DialogueStartEvent(dialogueInformation));
        }

        /// <summary>
        /// 현재 재생 중인 다이얼로그 종료 처리.
        /// </summary>
        /// <param name="evt">다이얼로그 종료 이벤트.</param>
        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            if (!_isPlaying || evt.DialogueSO != dialogueInformation)
            {
                return;
            }

            _isPlaying = false;

            if (_inputLocked)
            {
                _inputLocked = false;
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
            }
        }
    }
}
