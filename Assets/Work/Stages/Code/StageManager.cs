using LitMotion;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Fade;

namespace Work.Stages.Code
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private List<Stage> stageList = new List<Stage>();
        public Stage CurrentStage { get; private set; }

        private void Awake()
        {
            GeneratStage();
        }

        public Stage GetStage()
        {
            if (stageList.Count == 0)
            {
                Debug.LogError("Stage list is empty!");
                return null;
            }
            int randomIndex = Random.Range(0, stageList.Count);
            return stageList[randomIndex];
        }

        public void GeneratStage()
        {
            Stage selectedStage = GetStage();
            if (selectedStage == null) return;

            LMotion.Create(0f, 1f, 0.5f)
               .WithOnComplete(() => Bus<OnFadeEvent>.Raise(new OnFadeEvent(false)))
               .Bind(a => { })
               .AddTo(gameObject);

            Stage stage = Instantiate(selectedStage, Vector3.zero, Quaternion.identity);
            CurrentStage?.ExitStage();
            CurrentStage = stage;
            CurrentStage.EnterStage(this);
        }
    }
}