using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Work.Core.Utils.EventBus;
using Work.Fade;

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
            SceneManager.LoadScene("EnemyTestScene");
        }
        Bus<OnFadeCompletedEvent>.Events -= OnFadeCompleted;
    }
}
