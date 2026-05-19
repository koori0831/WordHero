using UnityEngine;
using Work.Core.Utils.EventBus;

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
            if (_stageManager != null)
            {
                _stageManager.RequestStageTransition(interactor, doorType);
            }
        }
    }
}
