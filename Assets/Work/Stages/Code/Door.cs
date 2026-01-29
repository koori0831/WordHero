using UnityEngine;
using UnityEngine.Events;

namespace Work.Stages.Code
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Stage stage;

        #region Test

        public GameObject testInteractor;

        [ContextMenu("Test")]
        public void Test()
        {
            Interact(testInteractor);
        }

        #endregion 

        public void Interact(GameObject interactor)
        {
            stage.HandleGoNextRoom(interactor);
        }
    }
}