using GondrLib.Dependencies;
using LitMotion;
using System.Collections.Generic;
using UnityEngine;
using Work.Chests.Code;
using Work.Combat.Code;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.ETC.LocationUI.Code;
using Work.Fade;
using Work.Input.Code;
using Work.Players.Code;
using Work.ProgressRate.Code;

namespace Work.Stages.Code
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private List<Stage> woodStageList = new List<Stage>();
        [SerializeField] private List<Stage> stoneStageList = new List<Stage>();
        [SerializeField] private List<Stage> ironStageList = new List<Stage>();
        [SerializeField] private List<Stage> goldStageList = new List<Stage>();
        [SerializeField] private List<Stage> questionStageList = new List<Stage>();
        [SerializeField] private Stage shopStage;
        [SerializeField] private Stage bossStage;
        [SerializeField] private List<int> openingShopCountList;
        [SerializeField] private int bossStageCount;

        [SerializeField] private Door doorPrefab;
        [SerializeField] private Chest chestPrefab;
        [SerializeField] private float chestSpawnDistance = 1.8f;
        public Door DoorPrefab => doorPrefab;

        [Inject] private Player _player;

        public bool IsOpeningShop
        {
            get
            {
                for (int i = 0; i < openingShopCountList.Count; i++)
                {
                    if (openingShopCountList[i] == CurrentStageCount)
                        return true;
                }
                return false;
            }
        }

        public bool IsNextStageInBossStage => bossStageCount == CurrentStageCount;

        private Dictionary<DoorType, List<Stage>> stages;

        private int _currentStageCount = 0;
        public int CurrentStageCount => _currentStageCount;

        private DoorType _currentStageDoorType = DoorType.Wood;
        private Chest _currentStageChest;

        public static Stage CurrentStage { get; private set; }

        private void Awake()
        {
            stages = new Dictionary<DoorType, List<Stage>>
            {
                { DoorType.Wood, woodStageList },
                { DoorType.Stone, stoneStageList },
                { DoorType.Iron, ironStageList },
                { DoorType.Gold, goldStageList },
                { DoorType.Question, questionStageList },
                { DoorType.Shop, new List<Stage> { shopStage } },
                { DoorType.Boss, new List<Stage> { bossStage } }
            };

            Bus<OnChestCreatEvent>.Events += HandleChestCreatEventEvent;
            Bus<StageProgressMapClosedEvent>.Events += HandleInitialProgressMapClosed;
        }

        /// <summary>
        /// 진행도 맵에 시작 연출 요청
        /// </summary>
        private void Start()
        {
            Bus<PlayInitialProgressMapEvent>.Raise(new PlayInitialProgressMapEvent());
        }

        private void OnDestroy()
        {
            Bus<OnChestCreatEvent>.Events -= HandleChestCreatEventEvent;
            Bus<StageProgressMapClosedEvent>.Events -= HandleInitialProgressMapClosed;
        }

        /// <summary>
        /// 스테이지 진행도 맵 닫기
        /// </summary>
        private void HandleInitialProgressMapClosed(StageProgressMapClosedEvent evt)
        {
            Bus<StageProgressMapClosedEvent>.Events -= HandleInitialProgressMapClosed;
            GenerateStage(DoorType.Wood);
        }

        public Stage GetStage(DoorType doorType) // 특정 상황에서 상점이나 보스 스테이지를 반환하도록 수정해야함
        {
            if (stages[doorType].Count == 0)
            {
                Debug.LogError("Stage list is empty!");
                return null;
            }


            int randomIndex = Random.Range(0, stages[doorType].Count);
            _currentStageCount++;
            return stages[doorType][randomIndex];
        }

        public void GenerateStage(DoorType doorType)
        {
            GameObject interactor = CurrentStage?.Interator;

            Stage selectedStage = GetStage(doorType);
            if (selectedStage == null) return;

            Stage stage = Instantiate(selectedStage, transform);
            CurrentStage?.ExitStage();
            CurrentStage = stage;
            _currentStageDoorType = doorType;
            _currentStageChest = null;
            if (interactor != null)
                interactor.transform.position = CurrentStage.SpawnPoint;
            CurrentStage.EnterStage(this);
            Bus<PlayLocationUIEvent>.Raise(new PlayLocationUIEvent());
        }

        private void HandleChestCreatEventEvent(OnChestCreatEvent evt)
        {
            if (_currentStageChest != null || CurrentStage == null)
                return;

            Bus<GetSkillEnergyEvent>.Raise(new GetSkillEnergyEvent(1f));


            CameraController.Instance.PlayImpulse(1f, 0.2f);
            Time.timeScale = 0.4f; //카메라 줌인 효과에 맞춰 시간 조정

            CameraController.Instance.ZoomIn(11f, duration: 0.25f, onComplete: () =>
            {
                Time.timeScale = 1f;
                CameraController.Instance.ZoomIn(1f, duration: 1f, onComplete: () =>
                {
                    if (CurrentStage is BattleStage battleStage)
                        CreatChest(battleStage.Doors, _player.transform);
                });
            });
        }

        public void DestroyChest()
        {
            if (_currentStageChest != null)
            {
                Destroy(_currentStageChest.gameObject);
                _currentStageChest = null;
            }
        }

        public void CreatChest(List<Door> doors, Transform targetTrm)
        {
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            //카메라 줌인 효과 추가 예정
            Vector3 spawnPosition = targetTrm.position + targetTrm.forward * chestSpawnDistance; //여기 땅위 랜덤위치로 조정 
            spawnPosition.y = targetTrm.position.y;

            Quaternion spawnRotation = Quaternion.LookRotation(targetTrm.position - spawnPosition);

            _currentStageChest = Instantiate(chestPrefab, spawnPosition, spawnRotation, CurrentStage.transform);
            _currentStageChest.Initialize(ConvertDoorTypeToChestType(_currentStageDoorType));

            _currentStageChest.cameraMovePosition = GetCameraPlaneBottomCenter(doors.ConvertAll(d => d.transform), Camera.main.transform);

            CameraController.Instance.MoveTo(_currentStageChest.transform.position, duration: 0.6f);
            CameraController.Instance.ZoomIn(15f, duration: 0.7f, onComplete: () =>
            {
                CameraController.Instance.ZoomIn(1f, duration: 1f, onComplete: () =>
                {
                    CameraController.Instance.ResetPosition(duration: 0.5f);
                    CameraController.Instance.ResetZoom(duration: 0.5f, onComplete: () =>
                    {
                        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                    });
                });
            });
        }


        public Vector3 GetCameraPlaneBottomCenter(
    List<Transform> targets,
    Transform cameraTransform)
        {
            if (targets == null || targets.Count == 0)
                return Vector3.zero;

            Vector3 camRight = cameraTransform.right;
            Vector3 camForward = cameraTransform.forward;

            camRight.y = 0f;
            camForward.y = 0f;

            camRight.Normalize();
            camForward.Normalize();

            float minRight = float.MaxValue;
            float maxRight = float.MinValue;
            float minForward = float.MaxValue;

            float avgY = 0f;

            for (int i = 0; i < targets.Count; i++)
            {
                Vector3 pos = targets[i].position;

                float r = Vector3.Dot(pos, camRight);
                float f = Vector3.Dot(pos, camForward);

                if (r < minRight) minRight = r;
                if (r > maxRight) maxRight = r;
                if (f < minForward) minForward = f;

                avgY += pos.y;
            }

            avgY /= targets.Count;

            float centerRight = (minRight + maxRight) * 0.5f;

            Vector3 bottomCenter =
                camRight * centerRight +
                camForward * minForward;

            bottomCenter.y = avgY;

            return bottomCenter;
        }

        private static bool IsNormalCombatStage(DoorType doorType)
        {
            return doorType == DoorType.Wood || doorType == DoorType.Stone || doorType == DoorType.Iron || doorType == DoorType.Gold;
        }

        private static ChestType ConvertDoorTypeToChestType(DoorType doorType)
        {
            return doorType switch
            {
                DoorType.Stone => ChestType.Stone,
                DoorType.Iron => ChestType.Iron,
                DoorType.Gold => ChestType.Gold,
                _ => ChestType.Wood
            };
        }

        public void DoorSpawn(List<Transform> doorPoints, ref List<Door> doors, bool isRandom = false)
        {
            int random = isRandom ? Random.Range(1, doorPoints.Count + 1) : doorPoints.Count;

            for (int i = 0; i < random; i++)
            {
                Transform x = doorPoints[i];
                Door door = Instantiate(DoorPrefab, x.position, Quaternion.identity);
                door.transform.parent = x;
                door.transform.localRotation = Quaternion.identity;
                door.DoorInit(CurrentStage);
                DoorType nextDoorType = (DoorType)Random.Range(0, 5);

                if (IsOpeningShop)
                {
                    nextDoorType = DoorType.Shop;
                    door.SetDoorType(nextDoorType);
                    doors.Add(door);
                    return;
                }

                if (IsNextStageInBossStage)
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
    }
}
