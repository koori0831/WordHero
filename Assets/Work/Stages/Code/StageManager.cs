using Cysharp.Threading.Tasks.Triggers;
using LitMotion;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Fade;

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
        public Door DoorPrefab => doorPrefab;

        public bool IsOpeningShop
        {
            get
            {
                for (int i = 0; i < openingShopCountList.Count; i++)
                {
                    if(openingShopCountList[i] == CurrentStageCount)
                        return true;
                }
                return false;
            }
        }

        public bool IsNextStageInBossStage => bossStageCount == CurrentStageCount;

        private Dictionary<DoorType, List<Stage>> stages;

        private int _currentStageCount = 0;
        public int CurrentStageCount => _currentStageCount;

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
            GeneratStage(DoorType.Wood);
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
            if(interactor != null)
            interactor.transform.position = CurrentStage.SpawnPoint;
            CurrentStage.EnterStage(this);
        }
    }
}