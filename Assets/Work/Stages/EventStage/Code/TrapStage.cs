using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Stages.Code;

namespace Work.Stages.EventStage.Code
{
    public class TrapStage : Stage
    {
        [field: SerializeField] public List<Transform> doorPoints { get; private set; } = new List<Transform>();
        protected List<Door> doors = new List<Door>();
        public List<Door> Doors => doors;

        [SerializeField] private Transform chestSpawnPoint;

        protected override void HandleStageClearEvent(StageClearEvent evt)
        {
            Bus<OnTrapDownEvent>.Raise(new OnTrapDownEvent());

            CameraController.Instance.ZoomIn(1f, duration: 1f,onComplete: () =>
            {
                CameraController.Instance.ResetPosition(duration: 0.75f);
                CameraController.Instance.ResetZoom(duration: 0.75f, onComplete: () =>
                {
                    Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                });
            });
        }

        public void DoorOpen()
        {
            Doors.ForEach(x =>
            {
                x.Open();
            });
        }

        public override void EnterStage(StageManager stageManager)
        {
            base.EnterStage(stageManager);
            Bus<OnTrapDownEvent>.Events += HandleStageClearEvent;
            _stageManager.DoorSpawn(doorPoints, ref doors, isRandom: true);
            _stageManager.CreatChest(Doors, chestSpawnPoint);
            DoorOpen();
        }

        private void HandleStageClearEvent(OnTrapDownEvent evt)
        {
            _stageManager.DestroyChest();
        }

        public override void ExitStage()
        {
            base.ExitStage();
            
        }
    }
}