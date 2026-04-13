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

        public void Interact(GameObject interactor)
        {
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