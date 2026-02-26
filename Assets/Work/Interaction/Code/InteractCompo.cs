using Code.Entities;
using System;
using System.Linq;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Interaction.Code
{
    public class InteractCompo : MonoBehaviour, IEntityComponent
    {
        public Entity Owner { get; private set; }

        [SerializeField] private float interactRange = 2f;
        [SerializeField] private Transform trm;



        public void InitCompo(Entity entity)
        {
            Owner = entity;
            Bus<InputInteractEvent>.Events += OnInteract;
        }

        private void OnDestroy()
        {
            Bus<InputInteractEvent>.Events -= OnInteract;
        }

        private void OnInteract(InputInteractEvent evt)
        {
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