using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Players.Code;
using Work.Stages.Code;

namespace Work.Stages.EventStage.Code
{
    public class AnzuFallStage : Stage
    {
        [Header("Door")]
        [SerializeField] private List<Transform> doorPoints = new List<Transform>();
        [SerializeField] private bool spawnDoorsAfterCutscene = true;
        [SerializeField] private bool openDoorsAfterCutscene;

        [Header("Player Path")]
        [SerializeField] private Transform centerPoint;
        [SerializeField] private Transform fallTargetPoint;
        [SerializeField] private Transform nestLandingPoint;

        [Header("Anzu")]
        [SerializeField] private Transform anzuActor;
        [SerializeField] private Transform anzuStartPoint;
        [SerializeField] private Transform anzuCatchPoint;
        [SerializeField] private Transform anzuNestPoint;
        [SerializeField] private Transform anzuCarryAnchor;

        [Header("Floor Collapse")]
        [SerializeField] private Animator floorAnimator;
        [SerializeField] private string floorCollapseTrigger = "Collapse";
        [SerializeField] private GameObject collapseFloorRoot;

        [Header("Timing")]
        [SerializeField] private float moveToCenterDuration = 0.8f;
        [SerializeField] private float floorCollapseWaitTime = 0.6f;
        [SerializeField] private float fallDuration = 1f;
        [SerializeField] private float anzuCatchDuration = 0.6f;
        [SerializeField] private float anzuCarryDuration = 1.4f;

        [Header("Movement Curves")]
        [SerializeField] private AnimationCurve moveToCenterCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve anzuMoveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private List<Door> _doors = new List<Door>();
        private Player _player;
        private CharacterController _playerController;
        private Transform _playerOriginalParent;
        private bool _wasPlayerControllerEnabled;
        private bool _isCutsceneRunning;
        private bool _isGameplayPhaseStarted;
        private bool _isCleared;
        private bool _isExiting;
        private bool _isPlayerAttachedToAnzu;

        public IReadOnlyList<Door> Doors => _doors;

        protected override void HandleStageClearEvent(StageClearEvent evt)
        {
            if (_isGameplayPhaseStarted == false || _isCleared)
            {
                return;
            }

            CompleteEventStage();
        }

        public override void EnterStage(StageManager stageManager)
        {
            base.EnterStage(stageManager);
            _player = FindFirstObjectByType<Player>();
            _playerController = _player != null ? _player.GetComponent<CharacterController>() : null;

            RunCutsceneAsync();
        }

        public override void ExitStage()
        {
            _isExiting = true;
            RestorePlayerControl(enableInput: false);
            base.ExitStage();
        }

        private async void RunCutsceneAsync()
        {
            if (_isCutsceneRunning || _player == null)
            {
                StartGameplayPhase();
                return;
            }

            _isCutsceneRunning = true;
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
            DisablePlayerControllerForCutscene();

            if (anzuActor != null && anzuStartPoint != null)
            {
                anzuActor.SetPositionAndRotation(anzuStartPoint.position, anzuStartPoint.rotation);
            }

            if (centerPoint != null)
            {
                await MoveTransformAsync(_player.transform, centerPoint.position, centerPoint.rotation, moveToCenterDuration, moveToCenterCurve);
            }

            if (_isExiting) return;

            PlayFloorCollapse();
            await WaitSecondsAsync(floorCollapseWaitTime);

            if (_isExiting) return;

            if (fallTargetPoint != null)
            {
                await MoveTransformAsync(_player.transform, fallTargetPoint.position, fallTargetPoint.rotation, fallDuration, fallCurve);
            }

            if (_isExiting) return;
            if (anzuActor != null && anzuCatchPoint != null)
            {
                await MoveTransformAsync(anzuActor, anzuCatchPoint.position, anzuCatchPoint.rotation, anzuCatchDuration, anzuMoveCurve);
            }

            if (_isExiting) return;
            AttachPlayerToAnzu();

            if (anzuActor != null && anzuNestPoint != null)
            {
                await MoveTransformAsync(anzuActor, anzuNestPoint.position, anzuNestPoint.rotation, anzuCarryDuration, anzuMoveCurve);
            }

            if (_isExiting) return;
            PlacePlayerAtNest();
            RestorePlayerControl(enableInput: true);

            _isCutsceneRunning = false;
            StartGameplayPhase();
        }

        private void PlayFloorCollapse()
        {
            if (floorAnimator != null && string.IsNullOrWhiteSpace(floorCollapseTrigger) == false)
            {
                floorAnimator.SetTrigger(floorCollapseTrigger);
            }

            if (collapseFloorRoot != null)
            {
                collapseFloorRoot.SetActive(false);
            }
        }

        private void StartGameplayPhase()
        {
            if (_isGameplayPhaseStarted)
            {
                return;
            }

            _isCutsceneRunning = false;
            _isGameplayPhaseStarted = true;

            if (spawnDoorsAfterCutscene)
            {
                SpawnDoorsIfNeeded();
            }

            if (openDoorsAfterCutscene)
            {
                CompleteEventStage();
            }
        }

        private void CompleteEventStage()
        {
            if (_isCleared)
            {
                return;
            }

            _isCleared = true;
            SpawnDoorsIfNeeded();
            OpenDoors();
        }

        private void SpawnDoorsIfNeeded()
        {
            if (_doors.Count > 0 || _stageManager == null || doorPoints == null || doorPoints.Count == 0)
            {
                return;
            }

            _stageManager.DoorSpawn(doorPoints, ref _doors, isRandom: true);
        }

        private void OpenDoors()
        {
            for (int i = 0; i < _doors.Count; i++)
            {
                if (_doors[i] != null)
                {
                    _doors[i].Open();
                }
            }
        }

        private void DisablePlayerControllerForCutscene()
        {
            if (_playerController == null)
            {
                return;
            }

            _wasPlayerControllerEnabled = _playerController.enabled;
            _playerController.enabled = false;
        }

        private void RestorePlayerControl(bool enableInput)
        {
            DetachPlayerFromAnzu(keepWorldPosition: true);

            if (_playerController != null)
            {
                _playerController.enabled = _wasPlayerControllerEnabled;
            }

            if (enableInput)
            {
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
            }
        }

        private void AttachPlayerToAnzu()
        {
            if (_player == null || anzuCarryAnchor == null)
            {
                return;
            }

            _playerOriginalParent = _player.transform.parent;
            _player.transform.SetParent(anzuCarryAnchor, worldPositionStays: false);
            _player.transform.localPosition = Vector3.zero;
            _player.transform.localRotation = Quaternion.identity;
            _isPlayerAttachedToAnzu = true;
        }

        private void DetachPlayerFromAnzu(bool keepWorldPosition)
        {
            if (_player == null || _isPlayerAttachedToAnzu == false)
            {
                return;
            }

            _player.transform.SetParent(_playerOriginalParent, keepWorldPosition);
            _isPlayerAttachedToAnzu = false;
        }

        private void PlacePlayerAtNest()
        {
            if (_player == null)
            {
                return;
            }

            DetachPlayerFromAnzu(keepWorldPosition: true);

            if (nestLandingPoint != null)
            {
                _player.transform.SetPositionAndRotation(nestLandingPoint.position, nestLandingPoint.rotation);
            }
        }

        private async Awaitable MoveTransformAsync(Transform target, Vector3 endPosition, Quaternion endRotation, float duration, AnimationCurve curve)
        {
            if (target == null)
            {
                return;
            }

            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;
            float safeDuration = Mathf.Max(0f, duration);
            if (safeDuration <= 0f)
            {
                target.SetPositionAndRotation(endPosition, endRotation);
                return;
            }

            float timer = 0f;
            while (timer < safeDuration && _isExiting == false)
            {
                float normalizedTime = Mathf.Clamp01(timer / safeDuration);
                float curveTime = curve != null ? curve.Evaluate(normalizedTime) : normalizedTime;
                target.SetPositionAndRotation(
                    Vector3.LerpUnclamped(startPosition, endPosition, curveTime),
                    Quaternion.SlerpUnclamped(startRotation, endRotation, curveTime));

                timer += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            if (_isExiting == false)
            {
                target.SetPositionAndRotation(endPosition, endRotation);
            }
        }

        private async Awaitable WaitSecondsAsync(float seconds)
        {
            float safeSeconds = Mathf.Max(0f, seconds);
            float timer = 0f;
            while (timer < safeSeconds && _isExiting == false)
            {
                timer += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
