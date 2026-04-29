using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Fade;
using Work.ProgressRate.Code;

namespace Work.Stages.Code
{

    public record struct OnNextRoomEvent(DoorType nextRoomType) : IEvent;

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
            if (evt.isFadeIn)
            {
                Bus<OnFadeCompletedEvent>.Events -= HandleFadeComplete;
                Bus<StageProgressMapClosedEvent>.Events += HandleStageProgressMapClosed;
                Bus<OnNextRoomEvent>.Raise(new OnNextRoomEvent(_nextRoomType));
            }
        }

        /// <summary>
        /// 스테이지 진행도 맵과의 연동
        /// </summary>
        protected virtual void HandleStageProgressMapClosed(StageProgressMapClosedEvent evt)
        {
            Bus<StageProgressMapClosedEvent>.Events -= HandleStageProgressMapClosed;
            if (_stageManager != null)
            {
                _stageManager.GenerateStage(_nextRoomType);
            }
        }
    }
}
