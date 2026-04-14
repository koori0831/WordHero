using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Fade;
using Work.Input.Code;
using Random = UnityEngine.Random;

namespace Work.Stages.Code
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] protected Transform spawnPoint;
        
        protected DoorType _nextRoomType;
        protected StageManager _stageManager;

        public Vector3 SpawnPoint => spawnPoint.position;
        public GameObject Interator { get; private set; }

        protected virtual void HandleStageClearEvent(StageClearEvent evt)
        {
        }

        public virtual void EnterStage(StageManager stageManager)
        {
            _stageManager = stageManager;
            Bus<StageClearEvent>.Events += HandleStageClearEvent;
        }

        public virtual void ExitStage()
        {
            Bus<StageClearEvent>.Events -= HandleStageClearEvent;
            Destroy(gameObject);
        }

        public virtual void HandleGoNextRoom(GameObject interactor, DoorType doorType)
        {
            Interator = interactor;
            _nextRoomType = doorType;
            Bus<OnFadeCompletedEvent>.Events += HandleFadeComplete;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));
        }

        protected virtual void HandleFadeComplete(OnFadeCompletedEvent evt)
        {
            Bus<OnFadeCompletedEvent>.Events -= HandleFadeComplete;
            _stageManager.GeneratStage(_nextRoomType);
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
        }
    }
}