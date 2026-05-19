using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;
using Work.Goods.Code;

namespace Work.Stages.Code
{
    public class BattleStage : Stage
    {
        [field:SerializeField] public List<Transform> doorPoints { get; private set; } = new List<Transform>();
        [SerializeField] private EnemyManager enemyManager;
        protected List<Door> doors = new List<Door>();
        public List<Door> Doors => doors;

        override protected void HandleStageClearEvent(StageClearEvent evt)
        {
            DoorOpen();
        }

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
            Bus<OnGoodsUIEvent>.Raise(new OnGoodsUIEvent(false));

            if (enemyManager == null)
            {
                enemyManager = GetComponentInChildren<EnemyManager>();
            }

            if (enemyManager != null)
            {
                enemyManager.Init(this);
            }
        }
    }
}
