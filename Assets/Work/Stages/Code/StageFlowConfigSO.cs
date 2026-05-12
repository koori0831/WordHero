using System.Collections.Generic;
using UnityEngine;

namespace Work.Stages.Code
{
    /// <summary>
    /// 스테이지 진행 규칙 설정
    /// </summary>
    [CreateAssetMenu(menuName = "Work/Stages/Stage Flow Config")]
    public class StageFlowConfigSO : ScriptableObject
    {
        [SerializeField] private DoorType initialDoorType = DoorType.Wood;
        [SerializeField] private int bossStageIndex = 10;
        [SerializeField] private List<int> shopStageIndices = new List<int>();
        [SerializeField] private List<DoorType> randomDoorCandidates = new List<DoorType>
        {
            DoorType.Wood,
            DoorType.Stone,
            DoorType.Iron,
            DoorType.Gold,
            DoorType.Question
        };

        /// <summary>
        /// 최초 방 타입
        /// </summary>
        public DoorType InitialDoorType => initialDoorType;

        /// <summary>
        /// 보스방 스테이지 번호
        /// </summary>
        public int BossStageIndex => Mathf.Max(1, bossStageIndex);

        /// <summary>
        /// 전체 스테이지 수
        /// </summary>
        public int TotalStageCount => BossStageIndex;

        /// <summary>
        /// 상점방 스테이지 번호 목록
        /// </summary>
        public IReadOnlyList<int> ShopStageIndices => shopStageIndices;

        /// <summary>
        /// 일반 랜덤 문 후보 목록
        /// </summary>
        public IReadOnlyList<DoorType> RandomDoorCandidates => randomDoorCandidates;

        /// <summary>
        /// 상점방 번호 포함 여부
        /// </summary>
        public bool ContainsShopStageIndex(int stageIndex)
        {
            for (int i = 0; i < shopStageIndices.Count; i++)
            {
                if (shopStageIndices[i] == stageIndex)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 일반 랜덤 문 타입
        /// </summary>
        public DoorType GetRandomDoorCandidate()
        {
            if (randomDoorCandidates.Count == 0)
            {
                return DoorType.Wood;
            }

            int randomIndex = Random.Range(0, randomDoorCandidates.Count);
            return randomDoorCandidates[randomIndex];
        }

        /// <summary>
        /// 보스방까지 남은 방 수
        /// </summary>
        public int GetRemainingStageCount(int currentStageCount)
        {
            return Mathf.Max(0, BossStageIndex - currentStageCount);
        }

        /// <summary>
        /// 보스방 제외 남은 일반 방 수
        /// </summary>
        public int GetRemainingNormalStageCount(int currentStageCount)
        {
            return Mathf.Max(0, BossStageIndex - currentStageCount - 1);
        }

        /// <summary>
        /// 에디터 값 보정
        /// </summary>
        private void OnValidate()
        {
            bossStageIndex = Mathf.Max(1, bossStageIndex);
        }
    }
}
