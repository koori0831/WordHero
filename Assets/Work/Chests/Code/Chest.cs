using UnityEngine;
using Work.Interaction.Code;
using Work.Players.Code;

namespace Work.Chests.Code
{
    public class Chest : MonoBehaviour, IInteractable
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
                // 연출 나오고


                CollectAction.Collect(player);
            }
        }
    }
}