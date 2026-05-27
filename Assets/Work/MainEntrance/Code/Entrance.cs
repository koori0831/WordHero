using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Core.Utils.EventBus;
using Work.Fade;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 투영 바빌론에서 인게임 씬으로 진입하는 출정 입구.
    /// </summary>
    public sealed class Entrance : MonoBehaviour, IInteractable
    {
        private const string IN_GAME_SCENE_NAME = "InGameScene";

        [SerializeField] private string requirementMessage = "에라에게 시작 무기군 2개를 먼저 선택 필요";

        private bool _isInteracting;

        /// <summary>
        /// 시작 무기 선택 완료 시 인게임 씬 전환 페이드 시작.
        /// </summary>
        /// <param name="interactor">상호작용을 수행한 오브젝트.</param>
        public void Interact(GameObject interactor)
        {
            if (_isInteracting)
            {
                return;
            }

            if (!RunLoadoutState.IsComplete)
            {
                Debug.Log(requirementMessage);
                Bus<RunLoadoutRequirementFailedEvent>.Raise(new RunLoadoutRequirementFailedEvent(requirementMessage));
                return;
            }

            _isInteracting = true;
            Bus<OnFadeCompletedEvent>.Events += OnFadeCompleted;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
        }

        /// <summary>
        /// 페이드 완료 후 인게임 씬 로드.
        /// </summary>
        /// <param name="evt">페이드 완료 이벤트.</param>
        private void OnFadeCompleted(OnFadeCompletedEvent evt)
        {
            Bus<OnFadeCompletedEvent>.Events -= OnFadeCompleted;

            if (evt.isFadeIn)
            {
                SceneManager.LoadScene(IN_GAME_SCENE_NAME);
                return;
            }

            _isInteracting = false;
        }

        /// <summary>
        /// 컴포넌트 제거 시 페이드 완료 구독 해제.
        /// </summary>
        private void OnDestroy()
        {
            Bus<OnFadeCompletedEvent>.Events -= OnFadeCompleted;
        }
    }
}
