using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Work.Enemies.Code.AttackModules
{
    public class AttackWarningDecal : MonoBehaviour
    {
        [SerializeField] private DecalProjector fillDecalProjector;
        [SerializeField] private DecalProjector backgroundDecalProjector;
        [SerializeField] private float defaultFadeInDuration = 0.12f;
        [SerializeField] private float defaultFadeOutDuration = 0.08f;
        [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private Color backgroundColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField, Range(0f, 1f)] private float fillMaxFade = 0.65f;
        [SerializeField, Range(0f, 1f)] private float backgroundMaxFade = 0.18f;
        [SerializeField] private bool destroyOnHide = true;
        [SerializeField] private float minimumFillLength = 0.02f;

        private Coroutine fadeCoroutine;
        private Coroutine fillCoroutine;
        private Material runtimeFillMaterial;
        private Material runtimeBackgroundMaterial;
        private Texture2D runtimeFillTexture;
        private Texture2D runtimeBackgroundTexture;
        private Color appliedFillColor;
        private Color appliedBackgroundColor;
        private Vector3 initialFillPivot;
        private Vector3 initialBackgroundPivot;
        private bool isInitialProjectorStateCached;

        private void Awake()
        {
            if (fillDecalProjector == null)
            {
                fillDecalProjector = GetComponentInChildren<DecalProjector>();
            }

            SetupRuntimeMaterial();
            CacheInitialProjectorState();
        }

        public void Show(Vector3 position, Quaternion rotation, Vector2 size, float projectionDepth)
        {
            Show(position, rotation, size, projectionDepth, defaultFadeInDuration);
        }

        public void Show(Vector3 position, Quaternion rotation, Vector2 size, float projectionDepth, float fadeInDuration)
        {
            ShowFilled(position, rotation, size, projectionDepth, Vector3.forward, fadeInDuration);
        }

        public void ShowFilled(Vector3 center, Quaternion rotation, Vector2 fullSize, float projectionDepth, Vector3 fillDirection, float fillDuration)
        {
            SetupRuntimeMaterial();
            CacheInitialProjectorState();
            gameObject.SetActive(true);
            SetFade(fillMaxFade, backgroundMaxFade);

            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            fillCoroutine = StartCoroutine(FillRoutine(center, rotation, fullSize, projectionDepth, fillDirection, fillDuration));
        }

        public void Hide()
        {
            Hide(defaultFadeOutDuration);
        }

        public void Hide(float fadeOutDuration)
        {
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
                fillCoroutine = null;
            }

            FadeTo(0f, fadeOutDuration, destroyOnHide);
        }

        public void HideImmediate()
        {
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
                fillCoroutine = null;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            Destroy(gameObject);
        }

        private void ApplyProjectorSize(Vector2 size, float projectionDepth)
        {
            if (fillDecalProjector == null)
                return;

            fillDecalProjector.size = new Vector3(size.x, size.y, projectionDepth);
        }

        private void CacheInitialProjectorState()
        {
            if (isInitialProjectorStateCached)
                return;

            if (fillDecalProjector != null)
            {
                initialFillPivot = fillDecalProjector.pivot;
            }

            if (backgroundDecalProjector != null)
            {
                initialBackgroundPivot = backgroundDecalProjector.pivot;
            }

            isInitialProjectorStateCached = true;
        }

        private void FadeTo(float targetFade, float duration, bool destroyAfterFade)
        {
            if (fillDecalProjector == null && backgroundDecalProjector == null)
            {
                if (destroyAfterFade)
                {
                    Destroy(gameObject);
                }

                return;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeRoutine(targetFade, duration, destroyAfterFade));
        }

        private void SetFade(float fillFade, float backgroundFade)
        {
            if (fillDecalProjector != null)
            {
                fillDecalProjector.fadeFactor = fillFade;
            }

            if (backgroundDecalProjector != null)
            {
                backgroundDecalProjector.fadeFactor = backgroundFade;
            }
        }

        private void SetupRuntimeMaterial()
        {
            runtimeFillMaterial = SetupProjectorMaterial(fillDecalProjector, runtimeFillMaterial, fillColor, ref runtimeFillTexture, ref appliedFillColor);
            runtimeBackgroundMaterial = SetupProjectorMaterial(backgroundDecalProjector, runtimeBackgroundMaterial, backgroundColor, ref runtimeBackgroundTexture, ref appliedBackgroundColor);
        }

        private Material SetupProjectorMaterial(DecalProjector projector, Material runtimeMaterial, Color color, ref Texture2D runtimeTexture, ref Color appliedColor)
        {
            if (projector == null || projector.material == null)
                return runtimeMaterial;

            if (runtimeMaterial == null)
            {
                runtimeMaterial = Instantiate(projector.material);
                projector.material = runtimeMaterial;
            }

            if (runtimeMaterial.HasProperty("_Color"))
                runtimeMaterial.SetColor("_Color", color);

            if (runtimeMaterial.HasProperty("_BaseColor"))
                runtimeMaterial.SetColor("_BaseColor", color);

            if (runtimeTexture == null)
            {
                runtimeTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                runtimeTexture.wrapMode = TextureWrapMode.Clamp;
                runtimeTexture.filterMode = FilterMode.Point;
            }

            if (appliedColor != color)
            {
                runtimeTexture.SetPixel(0, 0, color);
                runtimeTexture.Apply(false, false);
                appliedColor = color;
            }

            if (runtimeMaterial.HasProperty("Base_Map"))
                runtimeMaterial.SetTexture("Base_Map", runtimeTexture);

            if (runtimeMaterial.HasProperty("_BaseMap"))
                runtimeMaterial.SetTexture("_BaseMap", runtimeTexture);

            if (runtimeMaterial.HasProperty("_MainTex"))
                runtimeMaterial.SetTexture("_MainTex", runtimeTexture);

            return runtimeMaterial;
        }

        private IEnumerator FillRoutine(Vector3 center, Quaternion rotation, Vector2 fullSize, float projectionDepth, Vector3 fillDirection, float duration)
        {
            transform.SetPositionAndRotation(center, rotation);
            Vector3 normalizedDirection = fillDirection.sqrMagnitude > 0f ? fillDirection.normalized : transform.forward;
            float fullLength = Mathf.Max(fullSize.y, minimumFillLength);
            float elapsed = 0f;
            ApplyBackground(center, rotation, fullSize, projectionDepth);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                ApplyFill(center, rotation, fullSize.x, fullLength, projectionDepth, normalizedDirection, t);
                yield return null;
            }

            ApplyFill(center, rotation, fullSize.x, fullLength, projectionDepth, normalizedDirection, 1f);
            fillCoroutine = null;
        }

        private void ApplyFill(Vector3 center, Quaternion rotation, float width, float fullLength, float projectionDepth, Vector3 fillDirection, float progress)
        {
            float currentLength = Mathf.Max(fullLength * progress, minimumFillLength);
            float centerOffset = (currentLength - fullLength) * 0.5f;

            transform.SetPositionAndRotation(center, rotation);
            ApplyProjectorSize(new Vector2(width, currentLength), projectionDepth);

            if (fillDecalProjector != null)
            {
                ResetProjectorLocalTransform(fillDecalProjector);
                fillDecalProjector.pivot = new Vector3(initialFillPivot.x, centerOffset, initialFillPivot.z);
            }
        }

        private void ApplyBackground(Vector3 center, Quaternion rotation, Vector2 fullSize, float projectionDepth)
        {
            if (backgroundDecalProjector == null)
                return;

            ResetProjectorLocalTransform(backgroundDecalProjector);
            backgroundDecalProjector.size = new Vector3(fullSize.x, fullSize.y, projectionDepth);
            backgroundDecalProjector.pivot = initialBackgroundPivot;
        }

        private void ResetProjectorLocalTransform(DecalProjector projector)
        {
            if (projector == null || projector.transform == transform)
                return;

            projector.transform.localPosition = Vector3.zero;
            projector.transform.localRotation = Quaternion.identity;
        }

        private IEnumerator FadeRoutine(float targetFade, float duration, bool destroyAfterFade)
        {
            float startFillFade = fillDecalProjector != null ? fillDecalProjector.fadeFactor : 0f;
            float startBackgroundFade = backgroundDecalProjector != null ? backgroundDecalProjector.fadeFactor : 0f;
            float elapsed = 0f;

            if (duration <= 0f)
            {
                SetFade(targetFade, targetFade);
            }
            else
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    SetFade(Mathf.Lerp(startFillFade, targetFade, t), Mathf.Lerp(startBackgroundFade, targetFade, t));
                    yield return null;
                }

                SetFade(targetFade, targetFade);
            }

            fadeCoroutine = null;

            if (destroyAfterFade)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (runtimeFillMaterial != null)
            {
                Destroy(runtimeFillMaterial);
            }

            if (runtimeBackgroundMaterial != null)
            {
                Destroy(runtimeBackgroundMaterial);
            }

            if (runtimeFillTexture != null)
            {
                Destroy(runtimeFillTexture);
            }

            if (runtimeBackgroundTexture != null)
            {
                Destroy(runtimeBackgroundTexture);
            }
        }
    }
}
