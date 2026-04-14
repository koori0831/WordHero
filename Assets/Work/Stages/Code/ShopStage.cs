using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Goods.Code;

namespace Work.Stages.Code
{
    public class ShopStage : Stage
    {
        [field: SerializeField] public List<Transform> doorPoints { get; private set; } = new List<Transform>();
        protected List<Door> doors = new List<Door>();
        public List<Door> Doors => doors;

        public void DoorOpen()
        {
            Doors.ForEach(x =>
            {
                x.Open();
            });
        }

        override public void EnterStage(StageManager stageManager)
        {
            base.EnterStage(stageManager);
            _stageManager.DoorSpawn(doorPoints, ref doors, isRandom: true);
            Bus<OnGoodsUIEvent>.Raise(new OnGoodsUIEvent(true, -1));
            DoorOpen();
        }
    }
}