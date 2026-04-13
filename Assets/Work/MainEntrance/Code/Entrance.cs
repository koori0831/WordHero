using LitMotion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Core.Utils.EventBus;
using Work.ETC.LocationUI.Code;
using Work.Fade;

namespace Work.MainEntrance.Code
{
    public class Entrance : MonoBehaviour, IInteractable
    {
        public bool CanInteract => !_isInteracting;

        private bool _isInteracting;

        public void Interact(GameObject interactor)
        {
            if (!CanInteract) return;

            _isInteracting = true;
            Bus<OnFadeCompletedEvent>.Events += OnFadeCompleted;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
        }

        private void OnFadeCompleted(OnFadeCompletedEvent evt)
        {
            if (evt.isFadeIn)
            {
                SceneManager.LoadScene("InGameScene");
            }
            Bus<OnFadeCompletedEvent>.Events -= OnFadeCompleted;
        }
    }
}
