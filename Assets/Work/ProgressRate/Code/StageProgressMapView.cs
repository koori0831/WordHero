using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Work.Stages.Code;
using LitMotion;
using LitMotion.Extensions;
using Cysharp.Threading.Tasks;

namespace Work.ProgressRate.Code
{
    /// <summary>
    /// 스테이지 진행도 맵 뷰
    /// </summary>
    public class StageProgressMapView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject mapContainer;
        [SerializeField] private RectTransform contentRect; 
        [SerializeField] private RectTransform viewportRect; 
        [SerializeField] private StageProgressNode nodePrefab;
        [SerializeField] private Image dotPrefab; 

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

        private readonly List<StageProgressNode> _nodes = new List<StageProgressNode>();
        private readonly List<List<Image>> _dotGroups = new List<List<Image>>(); 
        private List<MotionHandle> _dotMotionHandles = new List<MotionHandle>();

        private void Awake()
        {
            mapContainer.SetActive(false);
            nodePrefab.gameObject.SetActive(false);
            dotPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            CancelProcess();
        }

        /// <summary>
        /// 최초 진행도 맵 연출
        /// </summary>
        public async UniTask PlayInitialAsync(IReadOnlyList<DoorType> roomHistory, int totalStageCount, CancellationToken cancellationToken)
        {
            CancelProcess();
            int validTotalStageCount = Mathf.Max(1, totalStageCount);
            InitializeMap(roomHistory, 0, validTotalStageCount, isInitial: true);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(cancellationToken);
            SetFocusImmediate(validTotalStageCount - 1);

            await UniTask.Delay(TimeSpan.FromSeconds(bossPreviewWaitTime), cancellationToken: cancellationToken);
            await FocusNodeAsync(0, returnToStartDuration, cancellationToken);
            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), cancellationToken: cancellationToken);

            _nodes[0].PlayActivateAnimation(animationDuration);

            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: cancellationToken);
            await CloseMapAsync(cancellationToken);
        }

        /// <summary>
        /// 다음 진행도 맵 연출
        /// </summary>
        public async UniTask PlayNextAsync(IReadOnlyList<DoorType> roomHistory, int nextIndex, int totalStageCount, CancellationToken cancellationToken)
        {
            CancelProcess();
            int validTotalStageCount = Mathf.Max(1, totalStageCount);
            if (nextIndex <= 0 || nextIndex >= validTotalStageCount)
            {
                return;
            }

            InitializeMap(roomHistory, nextIndex, validTotalStageCount, isInitial: false);
            mapContainer.SetActive(true);

            await WaitLayoutStabilization(cancellationToken);
            SetFocusImmediate(nextIndex - 1);

            await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAnimations), cancellationToken: cancellationToken);

            _nodes[nextIndex - 1].PlayCompleteAnimation(animationDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + 0.3f), cancellationToken: cancellationToken);

            if (nextIndex < validTotalStageCount)
            {
                UniTask moveTask = FocusNodeAsync(nextIndex, focusDuration, cancellationToken);
                if (nextIndex - 1 < _dotGroups.Count)
                {
                    List<Image> dots = _dotGroups[nextIndex - 1];
                    float delayPerDot = focusDuration / (dots.Count + 1);
                    for (int i = 0; i < dots.Count; i++)
                    {
                        int dotIndex = i;
                        PlayDotAnimation(dots[dotIndex]);
                        await UniTask.Delay(TimeSpan.FromSeconds(delayPerDot), cancellationToken: cancellationToken);
                    }
                }
                await moveTask;

                _nodes[nextIndex].PlayActivateAnimation(animationDuration);

                await UniTask.Delay(TimeSpan.FromSeconds(animationDuration + autoCloseDelay), cancellationToken: cancellationToken);
                await CloseMapAsync(cancellationToken);
            }
        }

        private UniTask CloseMapAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            mapContainer.SetActive(false);
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 진행도 맵 숨김 처리
        /// </summary>
        public void Hide()
        {
            CancelProcess();
            mapContainer.SetActive(false);
        }

        private void InitializeMap(IReadOnlyList<DoorType> roomHistory, int currentStageIndex, int totalStageCount, bool isInitial)
        {
            for (int i = 0; i < totalStageCount; i++)
            {
                if (_nodes.Count <= i) _nodes.Add(Instantiate(nodePrefab, contentRect));
                _nodes[i].transform.SetAsLastSibling();
                _nodes[i].gameObject.SetActive(true);

                bool isBoss = (i == totalStageCount - 1);
                DoorType roomType = i < roomHistory.Count ? roomHistory[i] : DoorType.None;
                
                bool isCompleted;
                bool isCurrent;
                if (isInitial) { isCompleted = false; isCurrent = false; }
                else
                {
                    isCompleted = i < currentStageIndex - 1;
                    isCurrent = i == currentStageIndex - 1;
                }
                
                _nodes[i].Setup(roomType, isBoss, isCompleted, isCurrent);

                if (i < totalStageCount - 1)
                {
                    if (_dotGroups.Count <= i) _dotGroups.Add(new List<Image>());
                    List<Image> currentDots = _dotGroups[i];
                    while (currentDots.Count < dotsPerLink) currentDots.Add(Instantiate(dotPrefab, contentRect));
                    
                    bool isDotActive = !isInitial && (i < currentStageIndex - 1);
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

        /// <summary>
        /// 진행도 맵 연출 정리
        /// </summary>
        public void CancelProcess()
        {
            CancelDotMotions();
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
