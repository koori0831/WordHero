using UnityEngine;
using UnityEngine.Events;
using Work.Enemies.Code;

namespace Work.Stages.Code
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Stage stage;
        [SerializeField] private EnemyManager enemyManmager;
        private bool _isInteract;
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
            if(_isInteract == true) return;
            if (enemyManmager.IsCanMoveRoom == false) return;

            _isInteract = true;
            stage.HandleGoNextRoom(interactor);
        }
    }
}