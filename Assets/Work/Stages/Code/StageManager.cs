using GondrLib.Dependencies;
using LitMotion;
using System.Collections.Generic;
using UnityEngine;
using Work.Chests.Code;
using Work.Core.Utils.Cameras;
using Work.Core.Utils.EventBus;
using Work.Fade;
using Work.Input.Code;
using Work.Players.Code;

namespace Work.Stages.Code
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private List<Stage> woodStageList = new List<Stage>();
        [SerializeField] private List<Stage> stoneStageList = new List<Stage>();
        [SerializeField] private List<Stage> ironStageList = new List<Stage>();
        [SerializeField] private List<Stage> goldStageList = new List<Stage>();
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
                { DoorType.Shop, new List<Stage> { shopStage } },
                { DoorType.Boss, new List<Stage> { bossStage } }
            };

            Bus<CombatStageClearEvent>.Events += HandleCombatStageClearEvent;
            GeneratStage(DoorType.Wood);
        }

        private void OnDestroy()
        {
            Bus<CombatStageClearEvent>.Events -= HandleCombatStageClearEvent;
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

        public void GeneratStage(DoorType doorType)
        {
            GameObject interactor = CurrentStage?.Interator;

            Stage selectedStage = GetStage(doorType);
            if (selectedStage == null) return;

            LMotion.Create(0f, 1f, 0.5f)
               .WithOnComplete(() => Bus<OnFadeEvent>.Raise(new OnFadeEvent(false)))
               .Bind(a => { })
               .AddTo(gameObject);

            Stage stage = Instantiate(selectedStage, transform);
            CurrentStage?.ExitStage();
            CurrentStage = stage;
            _currentStageDoorType = doorType;
            _currentStageChest = null;
            if (interactor != null)
                interactor.transform.position = CurrentStage.SpawnPoint;
            CurrentStage.EnterStage(this);
        }

        private void HandleCombatStageClearEvent(CombatStageClearEvent evt)
        {
            if (!IsNormalCombatStage(_currentStageDoorType))
            {
                Bus<StageClearEvent>.Raise(new StageClearEvent());
                return;
            }

            if (_currentStageChest != null || CurrentStage == null)
                return;

            if (chestPrefab == null || _player == null)
            {
                Debug.LogWarning("Chest spawn skipped: chestPrefab or Player injection is missing. Opening door directly.");
                Bus<StageClearEvent>.Raise(new StageClearEvent());
                return;
            }

            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));

            //카메라 줌인 효과 추가 예정
            CameraController.Instance.ZoomIn(11f, duration: 0.5f, onComplete: () =>
            {
                CameraController.Instance.ZoomIn(1f, duration: 1f, onComplete: () =>
                {
                    Vector3 spawnPosition = _player.transform.position + _player.transform.forward * chestSpawnDistance; //여기 땅위 랜덤위치로 조정 
                    spawnPosition.y = _player.transform.position.y;

                    Quaternion spawnRotation = Quaternion.LookRotation(_player.transform.position - spawnPosition);

                    _currentStageChest = Instantiate(chestPrefab, spawnPosition, spawnRotation, CurrentStage.transform);
                    _currentStageChest.Initialize(ConvertDoorTypeToChestType(_currentStageDoorType));
                    
                    Vector3 sumVec = Vector3.zero;

                    foreach (Door door in CurrentStage.Doors)
                    {
                        sumVec += door.transform.position;
                    }

                    sumVec /= CurrentStage.Doors.Count;
                    _currentStageChest.cameraMovePosition = sumVec;

                    CameraController.Instance.ZoomOut(12f, duration: 0.5f, onComplete: () =>
                    {
                        Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
                    });
                });
            });
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
    }
}
