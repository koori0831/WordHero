using LitMotion;
using System;
using UnityEngine;
using UnityEngine.Events;
using Work.Core.Utils.EventBus;
using Work.Fade;

namespace Work.Stages.Code
{
    public class Stage : MonoBehaviour
    {
        private GameObject _interactor;
        private StageManager _stageManager;

        public void EnterStage(StageManager stageManager)
        {
            _stageManager = stageManager;
        }
            
        public void ExitStage()
        {
            Destroy(gameObject);
        }

        public void HandleGoNextRoom(GameObject interactor)
        {
            _interactor = interactor;
            Bus<OnFadeCompletedEvent>.Events += HandleFadeComplete;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
            
        }

        private void HandleFadeComplete(OnFadeCompletedEvent evt)
        {
            _interactor.transform.position = Vector3.zero;
            Bus<OnFadeCompletedEvent>.Events -= HandleFadeComplete;
           
            _stageManager.GeneratStage();
        }
    }
}