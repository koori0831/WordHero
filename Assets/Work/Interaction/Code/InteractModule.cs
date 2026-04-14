using UnityEngine;
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

        public void Initialize(Agent owner)
        {
            Owner = owner;
            Bus<InputInteractEvent>.Events += OnInteract;

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
