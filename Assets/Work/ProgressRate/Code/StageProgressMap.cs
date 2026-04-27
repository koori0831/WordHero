using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;
using Work.Fade;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;

namespace Work.ProgressRate.Code
{
    public class StageProgressMap : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject mapContainer;
        [SerializeField] private RectTransform contentRect; 
        [SerializeField] private RectTransform viewportRect; 
        [SerializeField] private StageProgressNode nodePrefab;
        [SerializeField] private Image dotPrefab; 

        [Header("Stage Configuration")]
        [SerializeField] private int totalStageCount = 10; 
        [SerializeField] private int dotsPerLink = 3; 

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float focusDuration = 1.0f; 
        [SerializeField] private float dotAnimationDuration = 0.15f;
        [SerializeField] private float delayBetweenAnimations = 0.4f;
        [SerializeField] private float autoCloseDelay = 1.5f;
        
        [Header("Initial Preview Settings")]
        [SerializeField] private float bossPreviewWaitTime = 1.2f;
        [SerializeField] private float returnToStartDuration = 2.2f;

        private int _currentStageIndex = -1; // -1: 시작 전, 0: 0번방 진입 중...
        private DoorType _initialRoomType = DoorType.Wood;
        private readonly List<DoorType> _roomHistory = new List<DoorType>();
        private readonly List<StageProgressNode> _nodes = new List<StageProgressNode>();
        private readonly List<List<Image>> _dotGroups = new List<List<Image>>(); 
        
        private CancellationTokenSource _cts;

        private void Awake()
        {
            Bus<PlayInitialProgressMapEvent>.Events += HandleInitialEvent;
            Bus<OnNextRoomEvent>.Events += HandleNextRoomEvent;
            Bus<ResetStageProgressEvent>.Events += HandleResetEvent;
            Bus<SetInitialStageProgressEvent>.Events += HandleSetInitialEvent;
            
            mapContainer.SetActive(false);
            nodePrefab.gameObject.SetActive(false);
            dotPrefab.gameObject.SetActive(false);

            _roomHistory.Add(_initialRoomType);
        }

        private void OnDestroy()
        {
            Bus<PlayInitialProgressMapEvent>.Events -= HandleInitialEvent;
            Bus<OnNextRoomEvent>.Events -= HandleNextRoomEvent;
            Bus<ResetStageProgressEvent>.Events -= HandleResetEvent;
            Bus<SetInitialStageProgressEvent>.Events -= HandleSetInitialEvent;
            CancelProcess();
        }

        private void HandleSetInitialEvent(SetInitialStageProgressEvent evt)
        {
            _initialRoomType = evt.InitialRoomType;
            if (_currentStageIndex == -1)
            {
                _roomHistory.Clear();
                _roomHistory.Add(_initialRoomType);
            }
        }

        // --- 시퀀스 1: 게임 최초 시작 연출 (보스 프리뷰) ---
        private void HandleInitialEvent(PlayInitialProgressMapEvent evt)
        {
            _currentStageIndex = 0; // 이제 0번 방에 막 도착함
            CancelProcess();
            _cts = new CancellationTokenSource();
            PlayInitialSequenceAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid PlayInitialSequenceAsync(CancellationToken ct)
        {
            InitializeMap(isInitial: true);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(ct);
            SetFocusImmediate(totalStageCount - 1); // 보스부터 시작

            await UniTask.Delay(TimeSpan.FromSeconds(bossPreviewWaitTime), cancellationToken: ct);
            await FocusNodeAsync(0, returnToStartDuration, ct);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: ct);

            _nodes[0].PlayActivateAnimation(animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: ct);
            
            CloseMap();
        }

        // --- 시퀀스 2: 스테이지 전환 연출 (OnNextRoomEvent) ---
        private void HandleNextRoomEvent(OnNextRoomEvent evt)
        {
            // 인덱스 증가 및 역사 기록 (이전 연출이 0이었다면 이제 1로 증가)
            _currentStageIndex++;
            _roomHistory.Add(evt.nextRoomType);
            
            if (_currentStageIndex >= totalStageCount) return;

            CancelProcess();
            _cts = new CancellationTokenSource();
            PlayTransitionSequenceAsync(_currentStageIndex, _cts.Token).Forget();
        }

        private async UniTaskVoid PlayTransitionSequenceAsync(int nextIndex, CancellationToken ct)
        {
            InitializeMap(isInitial: false);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(ct);
            
            // 방금 클리어한 노드(nextIndex - 1)에 포커스
            SetFocusImmediate(nextIndex - 1);

            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAnimations), cancellationToken: ct);

            // 1. 클리어 노드 연출
            _nodes[nextIndex - 1].PlayCompleteAnimation(animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + 0.3f), cancellationToken: ct);

            // 2. 점선 이동 및 다음 노드 도착
            var moveTask = FocusNodeAsync(nextIndex, focusDuration, ct);
            if (nextIndex - 1 < _dotGroups.Count)
            {
                List<Image> dots = _dotGroups[nextIndex - 1];
                float delayPerDot = focusDuration / (dots.Count + 1);
                for (int i = 0; i < dots.Count; i++)
                {
                    int dotIndex = i;
                    LMotion.Create(new Color(1, 1, 1, 0.2f), Color.white, dotAnimationDuration).Bind(c => dots[dotIndex].color = c).AddTo(gameObject);
                    LMotion.Create(Vector3.one * 0.8f, Vector3.one * 1.3f, dotAnimationDuration * 0.5f).WithLoops(2, LoopType.Yoyo).Bind(s => dots[dotIndex].transform.localScale = s).AddTo(gameObject);
                    await UniTask.Delay(TimeSpan.FromSeconds(delayPerDot), cancellationToken: ct);
                }
            }
            await moveTask;

            // 3. 진입 노드 점등
            _nodes[nextIndex].PlayActivateAnimation(animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: ct);
            
            CloseMap();
        }

        private void HandleResetEvent(ResetStageProgressEvent evt)
        {
            _initialRoomType = evt.InitialRoomType;
            _currentStageIndex = -1;
            _roomHistory.Clear();
            _roomHistory.Add(_initialRoomType);
            CancelProcess();
            mapContainer.SetActive(false);
        }

        private void InitializeMap(bool isInitial)
        {
            for (int i = 0; i < totalStageCount; i++)
            {
                if (_nodes.Count <= i) _nodes.Add(Instantiate(nodePrefab, contentRect));
                _nodes[i].transform.SetAsLastSibling();
                _nodes[i].gameObject.SetActive(true);

                bool isBoss = (i == totalStageCount - 1);
                DoorType roomType = i < _roomHistory.Count ? _roomHistory[i] : DoorType.None;
                
                // [상태 결정]
                bool isCompleted;
                bool isCurrent;

                if (isInitial)
                {
                    isCompleted = false;
                    isCurrent = false;
                }
                else
                {
                    // 전환 연출 시작 시점에는 nextIndex - 1 노드가 '현재' 상태로 보여야 함
                    isCompleted = i < _currentStageIndex - 1;
                    isCurrent = i == _currentStageIndex - 1;
                }
                
                _nodes[i].Setup(roomType, isBoss, isCompleted, isCurrent);

                if (i < totalStageCount - 1)
                {
                    if (_dotGroups.Count <= i) _dotGroups.Add(new List<Image>());
                    List<Image> currentDots = _dotGroups[i];
                    while (currentDots.Count < dotsPerLink) currentDots.Add(Instantiate(dotPrefab, contentRect));
                    
                    bool isDotActive = !isInitial && (i < _currentStageIndex - 1);
                    for (int j = 0; j < currentDots.Count; j++)
                    {
                        currentDots[j].transform.SetAsLastSibling();
                        currentDots[j].gameObject.SetActive(true);
                        currentDots[j].color = isDotActive ? Color.white : new Color(1, 1, 1, 0.2f);
                    }
                }
            }
        }

        private async UniTask WaitLayoutStabilization(CancellationToken ct)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
            await UniTask.NextFrame(ct); 
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        }

        private async UniTask FocusNodeAsync(int index, float duration, CancellationToken ct)
        {
            if (index < 0 || index >= _nodes.Count) return;
            Vector2 targetPos = GetTargetAnchoredPosition(index);
            await LMotion.Create(contentRect.anchoredPosition, targetPos, duration).WithEase(Ease.OutCubic).Bind(pos => contentRect.anchoredPosition = pos).ToUniTask(ct);
        }

        private void SetFocusImmediate(int index)
        {
            if (index < 0 || index >= _nodes.Count) return;
            contentRect.anchoredPosition = GetTargetAnchoredPosition(index);
        }

        private Vector2 GetTargetAnchoredPosition(int index)
        {
            if (index < 0 || index >= _nodes.Count) return contentRect.anchoredPosition;
            RectTransform nodeRT = _nodes[index].GetComponent<RectTransform>();
            Vector3 viewportCenterWorld = viewportRect.TransformPoint(viewportRect.rect.center);
            Vector3 nodeCenterWorld = nodeRT.TransformPoint(nodeRT.rect.center);
            Vector3 worldDelta = viewportCenterWorld - nodeCenterWorld;
            Vector3 localDelta = contentRect.parent.InverseTransformVector(worldDelta);
            return new Vector2(contentRect.anchoredPosition.x + localDelta.x, contentRect.anchoredPosition.y);
        }

        private void CloseMap()
        {
            mapContainer.SetActive(false);
            Bus<StageProgressMapClosedEvent>.Raise(new StageProgressMapClosedEvent());
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(false));
        }

        private void CancelProcess()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}
