using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Fade;
using Random = UnityEngine.Random;

namespace Work.Stages.Code
{
    public class Stage : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private List<Transform> doorPoints = new List<Transform>();
        private List<Door> doors = new List<Door>();
        private DoorType _nextRoomType;
        private StageManager _stageManager;

        public Vector3 SpawnPoint => spawnPoint.position;
        public List<Door> Doors => doors;
        public GameObject Interator { get; private set; }

        private void DoorOpen(StageClearEvent evt)
        {
            doors.ForEach(x =>
            {
                x.Open();
            });
        }

        public void EnterStage(StageManager stageManager)
        {
            _stageManager = stageManager;
            DoorSpawn();
            Bus<StageClearEvent>.Events += DoorOpen;
        }

        private void DoorSpawn()
        {
            int random = doorPoints.Count;

            for (int i = 0; i < random; i++)
            {
                Transform x = doorPoints[i];
                Door door = Instantiate(_stageManager.DoorPrefab, x.position, Quaternion.identity);
                door.transform.parent = x;
                door.transform.localRotation = Quaternion.identity;
                door.DoorInit(this);
                DoorType nextDoorType = (DoorType)Random.Range(0, 4);

                if (_stageManager.IsOpeningShop)
                {
                    nextDoorType = DoorType.Shop;
                    door.SetDoorType(nextDoorType);
                    doors.Add(door);
                    return;
                }

                if (_stageManager.IsNextStageInBossStage)
                {
                    nextDoorType = DoorType.Boss;
                    door.SetDoorType(nextDoorType);
                    doors.Add(door);
                    return;
                }

                door.SetDoorType(nextDoorType);
                doors.Add(door);
            }
        }

        public void ExitStage()
        {
            Bus<StageClearEvent>.Events -= DoorOpen;
            Destroy(gameObject);
        }

        public void HandleGoNextRoom(GameObject interactor, DoorType doorType)
        {
            Interator = interactor;
            _nextRoomType = doorType;
            Bus<OnFadeCompletedEvent>.Events += HandleFadeComplete;
            Bus<OnFadeEvent>.Raise(new OnFadeEvent(true));

        }

        private void HandleFadeComplete(OnFadeCompletedEvent evt)
        {
            Bus<OnFadeCompletedEvent>.Events -= HandleFadeComplete;

            _stageManager.GeneratStage(_nextRoomType);
        }
    }
}