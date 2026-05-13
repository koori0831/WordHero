using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;
using Work.Fade;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Work.ProgressRate.Code
{
    /// <summary>
    /// 스테이지 진행도 맵 연출 흐름을 테스트하기 위한 스크립트
    /// </summary>
    public class StageProgressTester : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private DoorType testNextRoomType = DoorType.Wood;
        [SerializeField] private float fadeWaitTime = 1.0f;

        private void Update()
        {
            if (Keyboard.current == null) return;

            // T 키: 다음 스테이지 진행 시뮬레이션
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                RunTestSequence().Forget();
            }

            // R 키: 진행도 카운트 초기화
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Bus<ResetStageProgressEvent>.Raise(new ResetStageProgressEvent());
                Debug.Log("<color=yellow>[StageProgressTester]</color> Stage Progress Reset!");
            }
        }

        private async UniTaskVoid RunTestSequence()
        {
            Debug.Log("<color=cyan>[StageProgressTester]</color> Starting Test Sequence...");

            // 1. 페이드 인 (화면 어둡게)
            FadePresenter fadePresenter = FindFirstObjectByType<FadePresenter>();
            if (fadePresenter != null)
            {
                await fadePresenter.FadeAsync(true, CancellationToken.None);
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(fadeWaitTime));
            }

            // 2. 다음 방 진입 알림 및 진행도 맵 직접 재생
            Bus<OnNextRoomEvent>.Raise(new OnNextRoomEvent(testNextRoomType));
            StageProgressMapPresenter stageProgressMapPresenter = FindFirstObjectByType<StageProgressMapPresenter>();
            if (stageProgressMapPresenter != null)
            {
                await stageProgressMapPresenter.PlayNextAsync(testNextRoomType, CancellationToken.None);
            }
            if (fadePresenter != null)
            {
                await fadePresenter.FadeAsync(false, CancellationToken.None);
            }
            
            Debug.Log("<color=cyan>[StageProgressTester]</color> OnNextRoomEvent Raised.");
        }
    }
}
