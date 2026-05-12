using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;
using Work.Fade;
using Cysharp.Threading.Tasks;
using System;

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
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
            
            // 페이드 연출이 완료될 때까지 대기
            await UniTask.Delay(TimeSpan.FromSeconds(fadeWaitTime));

            // 2. 다음 방 진입 이벤트 발생 (StageProgressMap 트리거)
            // 내부 카운터가 1 증가하며 로드맵 연출이 시작됩니다.
            Bus<OnNextRoomEvent>.Raise(new OnNextRoomEvent(testNextRoomType));
            
            Debug.Log("<color=cyan>[StageProgressTester]</color> OnNextRoomEvent Raised.");
            
            // 로직상 StageProgressMap이 연출을 완료하면 
            // 스스로 OnFadeEvent(false)를 쏴서 화면을 밝게 만듭니다.
        }
    }
}
