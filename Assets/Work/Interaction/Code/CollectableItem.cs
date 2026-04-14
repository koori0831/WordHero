using UnityEngine;
using Work.Players.Code;

namespace Work.Interaction.Code
{
    public class CollectableItem : MonoBehaviour, IInteractable
    {

        [SerializeReference]
        public ICollectAction CollectAction;

        public void Start()
        {
            Debug.Assert(CollectAction != null);
            CollectAction.Initialize();
        }

        public void Interact(GameObject interactor)
        {

            if (interactor.TryGetComponent(out Player player))
            {
                CollectAction.Collect(player);
                Destroy(gameObject);
            }
        }
    }
}
