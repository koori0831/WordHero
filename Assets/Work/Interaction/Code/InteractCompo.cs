using System.Linq;
using UnityEngine;
using Work.Agents.Code;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Interaction.Code
{
    public class InteractCompo : MonoBehaviour, IAgentModule
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
            Debug.Log("Interact event received");

            Physics.OverlapSphere(trm.position, interactRange)
                .Select(col => col.GetComponent<IInteractable>())
                .Where(interactable => interactable != null)
                .ToList()
                .ForEach(interactable => interactable.Interact(Owner.gameObject));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(trm.position, interactRange);
        }
    }
}
