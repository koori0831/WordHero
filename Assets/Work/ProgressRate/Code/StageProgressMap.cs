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
        [SerializeField] private Image linePrefab;

        [Header("Stage Configuration")]
        [SerializeField] private int totalStageCount = 10; 

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float focusDuration = 0.7f;
        [SerializeField] private float delayBetweenAnimations = 0.3f;
        [SerializeField] private float autoCloseDelay = 1.2f;

        private int _currentStageIndex = -1; 
        private readonly List<StageProgressNode> _nodes = new List<StageProgressNode>();
        private readonly List<Image> _lines = new List<Image>();
        private CancellationTokenSource _cts;

        private void Awake()
        {
            Bus<OnNextRoomEvent>.Events += HandleNextRoomEvent;
            Bus<ResetStageProgressEvent>.Events += HandleResetEvent;
            
            mapContainer.SetActive(false);
            nodePrefab.gameObject.SetActive(false);
            linePrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<OnNextRoomEvent>.Events -= HandleNextRoomEvent;
            Bus<ResetStageProgressEvent>.Events -= HandleResetEvent;
            CancelProcess();
        }

        private void HandleNextRoomEvent(OnNextRoomEvent evt)
        {
            _currentStageIndex++;
            if (_currentStageIndex >= totalStageCount) return;

            CancelProcess();
            _cts = new CancellationTokenSource();
            
            InitializeMap();
            PlayMapSequenceAsync(_currentStageIndex, _cts.Token).Forget();
        }

        private void HandleResetEvent(ResetStageProgressEvent evt)
        {
            _currentStageIndex = -1;
            CancelProcess();
            mapContainer.SetActive(false);
        }

        private void InitializeMap()
        {
            for (int i = 0; i < totalStageCount; i++)
            {
                if (_nodes.Count <= i) _nodes.Add(Instantiate(nodePrefab, contentRect));
                _nodes[i].transform.SetAsLastSibling();
                _nodes[i].gameObject.SetActive(true);

                bool isBoss = (i == totalStageCount - 1);
                // i < _currentStageIndex - 1 : 이미 예전에 클리어한 방
                // i == _currentStageIndex - 1 : 방금 클리어한 방 (연출 전엔 하이라이트 유지)
                bool isCompleted = i < _currentStageIndex - 1;
                bool isCurrent = i == _currentStageIndex - 1;
                _nodes[i].Setup(isBoss, isCompleted, isCurrent);

                if (i < totalStageCount - 1)
                {
                    if (_lines.Count <= i) _lines.Add(Instantiate(linePrefab, contentRect));
                    _lines[i].transform.SetAsLastSibling();
                    _lines[i].gameObject.SetActive(true);
                    _lines[i].fillAmount = i < _currentStageIndex - 1 ? 1f : 0f;
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
            
            // 연출 시작 전 포커스: 방금 클리어한 방 (이전 방)
            SetFocusImmediate(Mathf.Max(0, _currentStageIndex - 1));
            mapContainer.SetActive(true);
        }

        private async UniTaskVoid PlayMapSequenceAsync(int nextIndex, CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenAnimations), cancellationToken: ct);

                // 1. 이전 노드 완료 애니메이션 (하이라이트 끄기 및 색상 변경)
                if (nextIndex > 0)
                {
                    _nodes[nextIndex - 1].PlayCompleteAnimation(animationDuration);
                    await UniTask.Delay(TimeSpan.FromSeconds(animationDuration * 0.5f), cancellationToken: ct);
                }

                // 2. 현재 노드로 이동 + 라인 차오르기
                var moveTask = FocusNodeAsync(nextIndex, focusDuration, ct);
                
                if (nextIndex > 0 && nextIndex <= _lines.Count)
                {
                    await LMotion.Create(0f, 1f, focusDuration)
                        .Bind(v => _lines[nextIndex - 1].fillAmount = v)
                        .ToUniTask(ct);
                }
                
                await moveTask;

                // 3. 현재 노드 점등
                if (nextIndex < _nodes.Count)
                {
                    _nodes[nextIndex].PlayActivateAnimation(animationDuration);
                    await UniTask.Delay(TimeSpan.FromSeconds(animationDuration), cancellationToken: ct);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(autoCloseDelay), cancellationToken: ct);
                CloseMap();
            }
            catch (OperationCanceledException) { }
            catch (Exception e) { Debug.LogException(e); }
        }

        private async UniTask FocusNodeAsync(int index, float duration, CancellationToken ct)
        {
            if (index < 0 || index >= _nodes.Count) return;
            Vector2 targetPos = GetTargetAnchoredPosition(index);
            
            await LMotion.Create(contentRect.anchoredPosition, targetPos, duration)
                .WithEase(Ease.OutCubic)
                .Bind(pos => contentRect.anchoredPosition = pos)
                .ToUniTask(ct);
        }

        private void SetFocusImmediate(int index)
        {
            if (index < 0 || index >= _nodes.Count) return;
            contentRect.anchoredPosition = GetTargetAnchoredPosition(index);
        }

        private Vector2 GetTargetAnchoredPosition(int index)
        {
            float nodeLocalX = _nodes[index].GetComponent<RectTransform>().anchoredPosition.x;
            float viewportWidth = viewportRect.rect.width;
            float targetX = -nodeLocalX + (viewportWidth * 0.5f);
            
            return new Vector2(targetX, contentRect.anchoredPosition.y);
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
