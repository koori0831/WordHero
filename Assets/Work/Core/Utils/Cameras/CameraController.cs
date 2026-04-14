using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Work.Core.Utils.Cameras
{
    public class CameraController : MonoBehaviour
    {
        public enum ImpulseStyle
        {
            Impact,
            Rumble
        }

        public static CameraController Instance { get; private set; }

        [field: SerializeField] public CinemachineCamera Camera { get; private set; }

        [SerializeField] private float defaultDuration = 0.35f;

        private Coroutine _zoomRoutine;
        private Coroutine _moveRoutine;
        private Coroutine _rotateRoutine;
        private Coroutine _povRoutine;
        private Coroutine _impulseRoutine;

        private CameraTarget _cachedTarget;
        private bool _isUsingMoveProxy;
        private Transform _moveProxyTarget;
        private CinemachinePositionComposer _positionComposer;
        private Vector3 _cachedComposerDamping;
        private bool _hasCachedComposerDamping;
        private bool _isMoveDampingOverrideActive;
        private Component _impulseSource;
        private object _activeImpulseEvent;
        private object _activeImpulseDefinition;
        private float _cachedImpulseDuration;
        private object _cachedImpulseShape;
        private bool _hasCachedImpulseDuration;
        private bool _hasCachedImpulseShape;

        private float _originalZoom;
        private float _originalPan;
        private float _originalTilt;
        private Quaternion _originalRotate;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (Camera == null)
            {
                Debug.LogWarning($"{nameof(CameraController)} has no CinemachineCamera assigned.", this);
            }

            _originalZoom = Camera != null ? GetCurrentZoom() : 0f;
            _originalRotate = Camera != null ? Camera.transform.rotation : Quaternion.identity;

            var panTilt = Camera != null ? Camera.GetComponent<CinemachinePanTilt>() : null;
            if (panTilt != null)
            {
                _originalPan = panTilt.PanAxis.Value;
                _originalTilt = panTilt.TiltAxis.Value;
            }

            CacheImpulseSource();
        }

        private void OnDisable()
        {
            StopImpulse(true, null);
            StopAllCameraRoutines();
            RestoreTrackingTarget();
            RestoreMoveDampingOverride();
        }

        private void OnDestroy()
        {
            if (_moveProxyTarget != null)
            {
                Destroy(_moveProxyTarget.gameObject);
                _moveProxyTarget = null;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void ZoomTo(float target, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;

            StopRoutine(ref _zoomRoutine);

            if (instant)
            {
                ApplyZoom(target);
                onComplete?.Invoke();
                return;
            }

            float animationDuration = ResolveDuration(duration);
            if (animationDuration <= 0f)
            {
                ApplyZoom(target);
                onComplete?.Invoke();
                return;
            }

            float start = GetCurrentZoom();
            _zoomRoutine = StartCoroutine(AnimateFloat(
                start,
                target,
                animationDuration,
                ApplyZoom,
                () => _zoomRoutine = null,
                onComplete));
        }

        public void ZoomIn(float amount, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;
            ZoomTo(GetCurrentZoom() - Mathf.Abs(amount), duration, instant, onComplete);
        }

        public void ZoomOut(float amount, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;
            ZoomTo(GetCurrentZoom() + Mathf.Abs(amount), duration, instant, onComplete);
        }

        public void ResetZoom(float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;
            ZoomTo(_originalZoom, duration, instant, onComplete);
        }

        public void MoveTo(Vector3 targetPosition, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;

            StopRoutine(ref _moveRoutine);
            UseMoveProxyTarget(targetPosition);
            BeginMoveDampingOverride();

            if (_moveProxyTarget == null)
            {
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            if (instant)
            {
                _moveProxyTarget.position = targetPosition;
                onComplete?.Invoke();
                return;
            }

            float animationDuration = ResolveDuration(duration);
            if (animationDuration <= 0f)
            {
                _moveProxyTarget.position = targetPosition;
                onComplete?.Invoke();
                return;
            }

            Vector3 start = _moveProxyTarget.position;
            _moveRoutine = StartCoroutine(AnimateVector3(
                start,
                targetPosition,
                animationDuration,
                x => _moveProxyTarget.position = x,
                () => { _moveRoutine = null; },
                onComplete));
        }

        public void ResetPosition(float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;

            StopRoutine(ref _moveRoutine);

            if (!_isUsingMoveProxy)
            {
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            Transform trackingTarget = _cachedTarget.TrackingTarget;
            if (trackingTarget == null)
            {
                RestoreTrackingTarget();
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            if (_moveProxyTarget == null)
            {
                RestoreTrackingTarget();
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            Vector3 targetPosition = trackingTarget.position;

            if (instant)
            {
                _moveProxyTarget.position = targetPosition;
                RestoreTrackingTarget();
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            float animationDuration = ResolveDuration(duration);
            if (animationDuration <= 0f)
            {
                _moveProxyTarget.position = targetPosition;
                RestoreTrackingTarget();
                RestoreMoveDampingOverride();
                onComplete?.Invoke();
                return;
            }

            Vector3 start = _moveProxyTarget.position;
            _moveRoutine = StartCoroutine(AnimateVector3(
                start,
                targetPosition,
                animationDuration,
                x => _moveProxyTarget.position = x,
                () =>
                {
                    _moveRoutine = null;
                    RestoreTrackingTarget();
                    RestoreMoveDampingOverride();
                },
                onComplete));
        }

        public void RotateTo(Quaternion targetRotation, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;

            StopRoutine(ref _rotateRoutine);

            if (instant)
            {
                Camera.transform.rotation = targetRotation;
                onComplete?.Invoke();
                return;
            }

            float animationDuration = ResolveDuration(duration);
            if (animationDuration <= 0f)
            {
                Camera.transform.rotation = targetRotation;
                onComplete?.Invoke();
                return;
            }

            Quaternion start = Camera.transform.rotation;
            _rotateRoutine = StartCoroutine(AnimateQuaternion(
                start,
                targetRotation,
                animationDuration,
                x => Camera.transform.rotation = x,
                () => _rotateRoutine = null,
                onComplete));
        }

        public void RotateToEuler(Vector3 targetEuler, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            RotateTo(Quaternion.Euler(targetEuler), duration, instant, onComplete);
        }

        public void ResetRotate(float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;
            RotateTo(_originalRotate, duration, instant, onComplete);
        }

        public void SetPov(float pan, float tilt, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;
            if (!TryGetPanTilt(onComplete, out var panTilt)) return;

            StopRoutine(ref _povRoutine);

            Vector2 target = new Vector2(pan, tilt);

            if (instant)
            {
                ApplyPov(panTilt, target);
                onComplete?.Invoke();
                return;
            }

            float animationDuration = ResolveDuration(duration);
            if (animationDuration <= 0f)
            {
                ApplyPov(panTilt, target);
                onComplete?.Invoke();
                return;
            }

            Vector2 start = new Vector2(panTilt.PanAxis.Value, panTilt.TiltAxis.Value);
            _povRoutine = StartCoroutine(AnimateVector2(
                start,
                target,
                animationDuration,
                x => ApplyPov(panTilt, x),
                () => _povRoutine = null,
                onComplete));
        }

        public void SetPov(Vector2 panTilt, float duration = -1f, bool instant = false, Action onComplete = null)
        {
            SetPov(panTilt.x, panTilt.y, duration, instant, onComplete);
        }

        public void ResetPov(float duration = -1f, bool instant = false, Action onComplete = null)
        {
            SetPov(_originalPan, _originalTilt, duration, instant, onComplete);
        }

        public void SetPriority(int priority, Action onComplete = null)
        {
            if (!TryGetCamera(onComplete)) return;

            Camera.Priority = priority;
            onComplete?.Invoke();
        }

        public void PlayImpulse(float force, float duration, Action onComplete = null)
        {
            PlayImpulse(force, duration, ImpulseStyle.Impact, onComplete);
        }

        public void PlayRumbleImpulse(float force, float duration, Action onComplete = null)
        {
            PlayImpulse(force, duration, ImpulseStyle.Rumble, onComplete);
        }

        public void PlayImpulse(float force, float duration, ImpulseStyle style, Action onComplete = null)
        {
            if (!TryGetImpulseSource(onComplete, out var impulseSource)) return;

            StopImpulse(true, null);

            float clampedDuration = Mathf.Max(0f, duration);
            Vector3 defaultVelocity = GetVector3MemberValue(impulseSource, "DefaultVelocity", Vector3.down);
            Vector3 velocity = defaultVelocity * force;

            object impulseDefinition = GetMemberValue(impulseSource, "ImpulseDefinition");
            if (impulseDefinition == null)
            {
                onComplete?.Invoke();
                return;
            }

            ApplyImpulseDefinitionOverrides(impulseDefinition, clampedDuration, style == ImpulseStyle.Rumble);

            var createMethod = impulseDefinition.GetType().GetMethod("CreateAndReturnEvent", new[] { typeof(Vector3), typeof(Vector3) });
            if (createMethod == null)
            {
                Debug.LogWarning($"{nameof(CameraController)} could not find CreateAndReturnEvent on impulse definition.", this);
                RestoreImpulseDefinitionOverrides();
                onComplete?.Invoke();
                return;
            }

            _activeImpulseEvent = createMethod.Invoke(impulseDefinition, new object[] { impulseSource.transform.position, velocity });

            if (_activeImpulseEvent == null)
            {
                RestoreImpulseDefinitionOverrides();
                onComplete?.Invoke();
                return;
            }

            object envelope = GetMemberValue(_activeImpulseEvent, "Envelope");
            if (envelope != null)
            {
                SetMemberValue(envelope, "AttackTime", 0f);
                SetMemberValue(envelope, "SustainTime", clampedDuration);
                SetMemberValue(envelope, "DecayTime", 0f);
                SetMemberValue(envelope, "HoldForever", false);
                SetMemberValue(_activeImpulseEvent, "Envelope", envelope);
            }

            if (clampedDuration <= 0f)
            {
                _activeImpulseEvent = null;
                RestoreImpulseDefinitionOverrides();
                onComplete?.Invoke();
                return;
            }

            _impulseRoutine = StartCoroutine(WaitImpulseComplete(clampedDuration, onComplete));
        }

        public void StopImpulse(bool forceNoDecay = true, Action onComplete = null)
        {
            StopRoutine(ref _impulseRoutine);

            if (_activeImpulseEvent != null)
            {
                float currentTime = GetImpulseCurrentTime(_activeImpulseEvent);
                InvokeMethod(_activeImpulseEvent, "Cancel", new object[] { currentTime, forceNoDecay });
                _activeImpulseEvent = null;
            }

            RestoreImpulseDefinitionOverrides();

            onComplete?.Invoke();
        }

        private bool TryGetCamera(Action onComplete)
        {
            if (Camera != null) return true;

            Debug.LogWarning($"{nameof(CameraController)} camera reference is missing.", this);
            onComplete?.Invoke();
            return false;
        }

        private float ResolveDuration(float duration)
        {
            return duration < 0f ? defaultDuration : duration;
        }

        private bool TryGetPanTilt(Action onComplete, out CinemachinePanTilt panTilt)
        {
            panTilt = null;
            if (Camera == null)
            {
                onComplete?.Invoke();
                return false;
            }

            panTilt = Camera.GetComponent<CinemachinePanTilt>();
            if (panTilt != null) return true;

            Debug.LogWarning($"{nameof(CameraController)} requires CinemachinePanTilt on Aim stage for POV control.", this);
            onComplete?.Invoke();
            return false;
        }

        private void CacheImpulseSource()
        {
            if (_impulseSource != null) return;

            if (Camera != null)
            {
                _impulseSource = Camera.GetComponent("CinemachineImpulseSource");
            }

            if (_impulseSource == null)
            {
                _impulseSource = GetComponent("CinemachineImpulseSource");
            }
        }

        private bool TryGetImpulseSource(Action onComplete, out Component impulseSource)
        {
            CacheImpulseSource();
            impulseSource = _impulseSource;

            if (impulseSource != null) return true;

            Debug.LogWarning($"{nameof(CameraController)} requires CinemachineImpulseSource on Camera or Controller object.", this);
            onComplete?.Invoke();
            return false;
        }

        private float GetImpulseCurrentTime(object impulseEvent)
        {
            if (impulseEvent == null) return Time.time;

            Type eventType = impulseEvent.GetType();
            Type managerType = eventType.Assembly.GetType("Unity.Cinemachine.CinemachineImpulseManager");
            if (managerType == null) return Time.time;

            var instanceProperty = managerType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            object manager = instanceProperty?.GetValue(null);
            if (manager == null) return Time.time;

            var currentTimeProperty = managerType.GetProperty("CurrentTime", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            object value = currentTimeProperty?.GetValue(manager);

            return value is float currentTime ? currentTime : Time.time;
        }

        private void ApplyImpulseDefinitionOverrides(object impulseDefinition, float duration, bool useRumbleShape)
        {
            RestoreImpulseDefinitionOverrides();

            _activeImpulseDefinition = impulseDefinition;

            object currentDuration = GetMemberValue(impulseDefinition, "ImpulseDuration");
            if (currentDuration is float originalDuration)
            {
                _cachedImpulseDuration = originalDuration;
                _hasCachedImpulseDuration = true;
            }

            SetMemberValue(impulseDefinition, "ImpulseDuration", duration);

            if (!useRumbleShape) return;

            object currentShape = GetMemberValue(impulseDefinition, "ImpulseShape");
            if (currentShape != null)
            {
                _cachedImpulseShape = currentShape;
                _hasCachedImpulseShape = true;

                object rumbleShape = ParseEnumValue(currentShape.GetType(), "Rumble");
                if (rumbleShape != null)
                {
                    SetMemberValue(impulseDefinition, "ImpulseShape", rumbleShape);
                }
            }
        }

        private void RestoreImpulseDefinitionOverrides()
        {
            if (_activeImpulseDefinition != null)
            {
                if (_hasCachedImpulseDuration)
                {
                    SetMemberValue(_activeImpulseDefinition, "ImpulseDuration", _cachedImpulseDuration);
                }

                if (_hasCachedImpulseShape)
                {
                    SetMemberValue(_activeImpulseDefinition, "ImpulseShape", _cachedImpulseShape);
                }
            }

            _activeImpulseDefinition = null;
            _cachedImpulseShape = null;
            _hasCachedImpulseDuration = false;
            _hasCachedImpulseShape = false;
        }

        private static object ParseEnumValue(Type enumType, string enumName)
        {
            if (enumType == null || !enumType.IsEnum || string.IsNullOrEmpty(enumName)) return null;

            try
            {
                return Enum.Parse(enumType, enumName);
            }
            catch
            {
                return null;
            }
        }

        private static object GetMemberValue(object target, string memberName)
        {
            if (target == null) return null;

            Type type = target.GetType();
            var field = type.GetField(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) return field.GetValue(target);

            var property = type.GetProperty(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return property?.CanRead == true ? property.GetValue(target) : null;
        }

        private static bool SetMemberValue(object target, string memberName, object value)
        {
            if (target == null) return false;

            Type type = target.GetType();
            var field = type.GetField(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
                return true;
            }

            var property = type.GetProperty(memberName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (property?.CanWrite == true)
            {
                property.SetValue(target, value);
                return true;
            }

            return false;
        }

        private static object InvokeMethod(object target, string methodName, object[] args)
        {
            if (target == null) return null;

            Type type = target.GetType();
            var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return method?.Invoke(target, args);
        }

        private static Vector3 GetVector3MemberValue(object target, string memberName, Vector3 defaultValue)
        {
            object value = GetMemberValue(target, memberName);
            return value is Vector3 vector ? vector : defaultValue;
        }

        private float GetCurrentZoom()
        {
            var lens = Camera.Lens;
            return lens.Orthographic ? lens.OrthographicSize : lens.FieldOfView;
        }

        private void ApplyZoom(float value)
        {
            var lens = Camera.Lens;

            if (lens.Orthographic)
            {
                lens.OrthographicSize = Mathf.Max(0.01f, value);
            }
            else
            {
                lens.FieldOfView = Mathf.Clamp(value, 1f, 179f);
            }

            Camera.Lens = lens;
        }

        private static void ApplyPov(CinemachinePanTilt panTilt, Vector2 value)
        {
            var panAxis = panTilt.PanAxis;
            panAxis.Value = panAxis.ClampValue(value.x);
            panTilt.PanAxis = panAxis;

            var tiltAxis = panTilt.TiltAxis;
            tiltAxis.Value = tiltAxis.ClampValue(value.y);
            panTilt.TiltAxis = tiltAxis;
        }

        private void StopAllCameraRoutines()
        {
            StopRoutine(ref _zoomRoutine);
            StopRoutine(ref _moveRoutine);
            StopRoutine(ref _rotateRoutine);
            StopRoutine(ref _povRoutine);
            StopRoutine(ref _impulseRoutine);
        }

        private void UseMoveProxyTarget(Vector3 fallbackPosition)
        {
            if (Camera == null) return;

            EnsureMoveProxyTarget();
            if (_moveProxyTarget == null) return;

            if (_isUsingMoveProxy) return;

            _cachedTarget = Camera.Target;

            _moveProxyTarget.position = _cachedTarget.TrackingTarget != null
                ? _cachedTarget.TrackingTarget.position
                : fallbackPosition;

            var target = Camera.Target;
            target.TrackingTarget = _moveProxyTarget;
            Camera.Target = target;
            Camera.PreviousStateIsValid = false;

            _isUsingMoveProxy = true;
        }

        private void RestoreTrackingTarget()
        {
            if (!_isUsingMoveProxy || Camera == null) return;

            var target = Camera.Target;
            target.TrackingTarget = _cachedTarget.TrackingTarget;
            target.LookAtTarget = _cachedTarget.LookAtTarget;
            target.CustomLookAtTarget = _cachedTarget.CustomLookAtTarget;
            Camera.Target = target;
            Camera.PreviousStateIsValid = false;
            _isUsingMoveProxy = false;
        }

        private void EnsureMoveProxyTarget()
        {
            if (_moveProxyTarget != null) return;

            var proxy = new GameObject("Camera Move Proxy")
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            _moveProxyTarget = proxy.transform;
        }

        private bool TryGetPositionComposer(out CinemachinePositionComposer composer)
        {
            composer = _positionComposer;
            if (composer != null) return true;

            if (Camera == null) return false;

            composer = Camera.GetComponent<CinemachinePositionComposer>();
            _positionComposer = composer;
            return composer != null;
        }

        private void BeginMoveDampingOverride()
        {
            if (_isMoveDampingOverrideActive) return;
            if (!TryGetPositionComposer(out var composer)) return;

            if (!_hasCachedComposerDamping)
            {
                _cachedComposerDamping = composer.Damping;
                _hasCachedComposerDamping = true;
            }

            composer.Damping = Vector3.zero;
            _isMoveDampingOverrideActive = true;
        }

        private void RestoreMoveDampingOverride()
        {
            if (!_isMoveDampingOverrideActive) return;
            if (!TryGetPositionComposer(out var composer))
            {
                _isMoveDampingOverrideActive = false;
                return;
            }

            if (_hasCachedComposerDamping)
            {
                composer.Damping = _cachedComposerDamping;
            }

            _isMoveDampingOverrideActive = false;
        }

        private void StopRoutine(ref Coroutine routine)
        {
            if (routine == null) return;

            StopCoroutine(routine);
            routine = null;
        }

        private IEnumerator AnimateFloat(float start, float target, float duration, Action<float> apply, Action onFinished, Action onComplete)
        {
            float elapsed = 0f;
            apply(start);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Ease01(Mathf.Clamp01(elapsed / duration));
                apply(Mathf.Lerp(start, target, t));
                yield return null;
            }

            apply(target);
            onFinished?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator AnimateVector3(Vector3 start, Vector3 target, float duration, Action<Vector3> apply, Action onFinished, Action onComplete)
        {
            float elapsed = 0f;
            apply(start);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Ease01(Mathf.Clamp01(elapsed / duration));
                apply(Vector3.Lerp(start, target, t));
                yield return null;
            }

            apply(target);
            onFinished?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator AnimateQuaternion(Quaternion start, Quaternion target, float duration, Action<Quaternion> apply, Action onFinished, Action onComplete)
        {
            float elapsed = 0f;
            apply(start);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Ease01(Mathf.Clamp01(elapsed / duration));
                apply(Quaternion.Slerp(start, target, t));
                yield return null;
            }

            apply(target);
            onFinished?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator AnimateVector2(Vector2 start, Vector2 target, float duration, Action<Vector2> apply, Action onFinished, Action onComplete)
        {
            float elapsed = 0f;
            apply(start);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Ease01(Mathf.Clamp01(elapsed / duration));
                apply(Vector2.Lerp(start, target, t));
                yield return null;
            }

            apply(target);
            onFinished?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator WaitImpulseComplete(float duration, Action onComplete)
        {
            yield return new WaitForSeconds(duration);
            _impulseRoutine = null;
            _activeImpulseEvent = null;
            RestoreImpulseDefinitionOverrides();
            onComplete?.Invoke();
        }

        private static float Ease01(float t)
        {
            return t * t * (3f - (2f * t));
        }
    }
}
