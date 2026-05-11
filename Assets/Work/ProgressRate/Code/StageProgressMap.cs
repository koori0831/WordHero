using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Work.Core.Utils.EventBus;
using Work.Stages.Code;
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

        private int _currentStageIndex = -1; 
        private DoorType _initialRoomType = DoorType.Wood;
        private readonly List<DoorType> _roomHistory = new List<DoorType>();
        private readonly List<StageProgressNode> _nodes = new List<StageProgressNode>();
        private readonly List<List<Image>> _dotGroups = new List<List<Image>>(); 
        private List<MotionHandle> _dotMotionHandles = new List<MotionHandle>();
        
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

        private void HandleInitialEvent(PlayInitialProgressMapEvent evt)
        {
            _currentStageIndex = 0; 
            CancelProcess();
            _cts = new CancellationTokenSource();
            PlayInitialSequenceAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid PlayInitialSequenceAsync(CancellationToken ct)
        {
            InitializeMap(isInitial: true);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(ct);
            SetFocusImmediate(totalStageCount - 1); 

            await UniTask.Delay(TimeSpan.FromSeconds(bossPreviewWaitTime), cancellationToken: ct);
            await FocusNodeAsync(0, returnToStartDuration, ct);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: ct);

            _nodes[0].PlayActivateAnimation(animationDuration);
            
            // 인지 시간 대기 후 종료 시퀀스 실행
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: ct);
            await CloseMapAsync(ct);
        }

        private void HandleNextRoomEvent(OnNextRoomEvent evt)
        {
            _roomHistory.Add(evt.nextRoomType);
            _currentStageIndex++;
            
            // 먹통 방지: 범위를 벗어나면 즉시 페이드 해제
            if (_currentStageIndex >= totalStageCount) 
            {
                Bus<StageProgressMapClosedEvent>.Raise(new StageProgressMapClosedEvent());
                return;
            }

            CancelProcess();
            _cts = new CancellationTokenSource();
            PlayTransitionSequenceAsync(_currentStageIndex, _cts.Token).Forget();
        }

        private async UniTaskVoid PlayTransitionSequenceAsync(int nextIndex, CancellationToken ct)
        {
            InitializeMap(isInitial: false);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(ct);
            SetFocusImmediate(nextIndex - 1);

            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAnimations), cancellationToken: ct);

            _nodes[nextIndex - 1].PlayCompleteAnimation(animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + 0.3f), cancellationToken: ct);

            if (nextIndex < totalStageCount)
            {
                UniTask moveTask = FocusNodeAsync(nextIndex, focusDuration, ct);
                if (nextIndex - 1 < _dotGroups.Count)
                {
                    List<Image> dots = _dotGroups[nextIndex - 1];
                    float delayPerDot = focusDuration / (dots.Count + 1);
                    for (int i = 0; i < dots.Count; i++)
                    {
                        int dotIndex = i;
                        PlayDotAnimation(dots[dotIndex]);
                        await UniTask.Delay(TimeSpan.FromSeconds(delayPerDot), cancellationToken: ct);
                    }
                }
                await moveTask;

                _nodes[nextIndex].PlayActivateAnimation(animationDuration);
                
                // 인지 시간 대기 후 종료 시퀀스 실행
                await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: ct);
                await CloseMapAsync(ct);
            }
        }

        private UniTask CloseMapAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            mapContainer.SetActive(false);
            Bus<StageProgressMapClosedEvent>.Raise(new StageProgressMapClosedEvent());
            return UniTask.CompletedTask;
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
                
                bool isCompleted;
                bool isCurrent;
                if (isInitial) { isCompleted = false; isCurrent = false; }
                else
                {
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
                        currentDots[j].transform.localScale = Vector3.one;
                    }
                }
            }
        }

        private void PlayDotAnimation(Image dot)
        {
            MotionHandle colorHandle = LMotion.Create(new Color(1, 1, 1, 0.2f), Color.white, dotAnimationDuration)
                .Bind(color => dot.color = color)
                .AddTo(gameObject);

            MotionHandle scaleHandle = LMotion.Create(Vector3.one * 0.8f, Vector3.one * 1.3f, dotAnimationDuration * 0.5f)
                .WithLoops(2, LoopType.Yoyo)
                .WithOnComplete(() => dot.transform.localScale = Vector3.one)
                .Bind(scale => dot.transform.localScale = scale)
                .AddTo(gameObject);

            _dotMotionHandles.Add(colorHandle);
            _dotMotionHandles.Add(scaleHandle);
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

        private void CancelProcess()
        {
            CancelDotMotions();
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private void CancelDotMotions()
        {
            for (int i = 0; i < _dotMotionHandles.Count; i++)
            {
                _dotMotionHandles[i].TryCancel();
            }

            _dotMotionHandles.Clear();
        }
    }
}
