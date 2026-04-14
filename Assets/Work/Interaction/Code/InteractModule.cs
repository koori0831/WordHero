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

        public void Initialize(Agent owner)
        {
            Owner = owner;
            Bus<InputInteractEvent>.Events += OnInteract;
        }

        private void OnDestroy()
        {
            Bus<InputInteractEvent>.Events -= OnInteract;
        }

        private void OnInteract(InputInteractEvent evt)
        {

            if (Owner == null) return;

            Vector3 center = trm != null ? trm.position : Owner.transform.position;
            Collider[] colliders = Physics.OverlapSphere(center, interactRange);

            IInteractable nearest = null;
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

            nearest?.Interact(Owner.gameObject);
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
