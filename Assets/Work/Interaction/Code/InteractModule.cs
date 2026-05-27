using UnityEngine;
using TMPro;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Interaction.Code
{
    public class InteractModule : MonoBehaviour, IAgentModule
    {
        public Agent Owner { get; private set; }

        [SerializeField] private float interactRange = 2f;
        [SerializeField] private Transform trm;
        [SerializeField] private CanvasGroup interactUI;
        [SerializeField] private float uiRightOffset = 1f;
        [SerializeField] private float uiUpOffset = 1f;
        [SerializeField] private bool createPromptIfMissing = true;
        [SerializeField] private string promptText = "상호작용";
        [SerializeField] private Vector2 generatedPromptSize = new Vector2(220f, 60f);
        [SerializeField] private float generatedPromptScale = 0.01f;

        public void Initialize(Agent owner)
        {
            Owner = owner;
            Bus<InputInteractEvent>.Events += OnInteract;

            if (interactUI == null && createPromptIfMissing)
            {
                CreateDefaultInteractPrompt();
            }

            if (interactUI != null)
            {
                interactUI.alpha = 0f;
            }
        }

        private void OnDestroy()
        {
            Bus<InputInteractEvent>.Events -= OnInteract;
        }

        private void OnInteract(InputInteractEvent evt)
        {
            if (Owner == null) return;

            if (TryGetNearestInteractable(out IInteractable nearest))
            {
                nearest.Interact(Owner.gameObject);
            }
        }

        private void Update()
        {
            if (Owner == null || interactUI == null)
            {
                return;
            }

            bool canInteract = TryGetNearestInteractable(out _);
            interactUI.alpha = canInteract ? 1f : 0f;

            if (!canInteract)
            {
                return;
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                return;
            }

            Transform uiTransform = interactUI.transform;
            uiTransform.position = Owner.transform.position
                                   + (Owner.transform.right * uiRightOffset)
                                   + (Owner.transform.up * uiUpOffset);
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraUp = cam.transform.up;
            uiTransform.rotation = Quaternion.LookRotation(cameraForward, cameraUp);
        }

        private bool TryGetNearestInteractable(out IInteractable nearest)
        {
            nearest = null;

            Vector3 center = trm != null ? trm.position : Owner.transform.position;
            Collider[] colliders = Physics.OverlapSphere(center, interactRange);
            float nearestSqrDistance = float.MaxValue;

            for (int i = 0; i < colliders.Length; i++)
            {
                IInteractable interactable = colliders[i].GetComponentInParent<IInteractable>();
                if (interactable == null) continue;

                float sqrDistance = (colliders[i].transform.position - center).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearest = interactable;
                }
            }

            return nearest != null;
        }

        /// <summary>
        /// 별도 프롬프트가 연결되지 않은 씬에서 사용할 기본 상호작용 표시 생성
        /// </summary>
        private void CreateDefaultInteractPrompt()
        {
            GameObject canvasObject = new GameObject("InteractPrompt", typeof(RectTransform));
            canvasObject.transform.SetParent(transform, false);
            canvasObject.transform.localScale = Vector3.one * generatedPromptScale;

            RectTransform canvasRectTransform = canvasObject.GetComponent<RectTransform>();
            canvasRectTransform.sizeDelta = generatedPromptSize;

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            interactUI = canvasObject.AddComponent<CanvasGroup>();
            interactUI.interactable = false;
            interactUI.blocksRaycasts = false;

            GameObject textObject = new GameObject("Text", typeof(RectTransform));
            textObject.transform.SetParent(canvasObject.transform, false);

            RectTransform textRectTransform = textObject.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = promptText;
            text.fontSize = 36f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (trm != null)
            {
                Gizmos.DrawWireSphere(trm.position, interactRange);
            }
        }
    }
}
