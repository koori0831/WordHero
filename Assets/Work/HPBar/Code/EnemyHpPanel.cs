using UnityEngine;
using Work.Combat.Code;
using Work.Core.Utils.EventBus;
using Work.Cursor.Code;
using Work.Information.Code;

namespace Work.HPBar.Code
{
    //
    public class EnemyHpPanel : MonoBehaviour
    {
        [SerializeField] private HPField hpField;
        [SerializeField] private NameField nameField;
        [SerializeField] private LmmunityInformationField lmmunityInformationField;

        private EnemyInfoDataSO currentEnemyInfoData;

        public void Awake()
        {
            hpField.Disable();
            Bus<InfoDataEvent>.Events += OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events += OnHideInfoDataEvent;
        }

        private void OnDestroy()
        {
            Bus<InfoDataEvent>.Events -= OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events -= OnHideInfoDataEvent;
        }

        private void OnHideInfoDataEvent(HideInfoDataEvent evt)
        {
            if (currentEnemyInfoData != null)
                currentEnemyInfoData.EnemyHpValue.OnHpChanged -= HandleHPChangeEvent;
            AllDisable();
            currentEnemyInfoData = null;
        }

        private void OnInfoDataEvent(InfoDataEvent evt)
        {
            if (currentEnemyInfoData != null) return;
            if (!(evt.Info is EnemyInfoDataSO data)) return;
            currentEnemyInfoData = data;
            currentEnemyInfoData.EnemyHpValue.OnHpChanged += HandleHPChangeEvent;
            currentEnemyInfoData.EnemyHpValue.OnDead += HandleDeadEvent;
            currentEnemyInfoData.StatusValue.OnstateusChangeEvent += HandleStatusChangeEvent;
            AllEnable();
        }

        private void HandleStatusChangeEvent(StatusType type, bool state)
        {
            //if (state)
            //    lmmunityInformationField.AddStatus(type);
            //else
            //    lmmunityInformationField.RemoveStatus(type);

            string statusText = GetLmmunityIfo();
            lmmunityInformationField.SetStatusText(statusText);

        }

        private string GetLmmunityIfo()
        {
            string statusText = "면역 없음";

            if (currentEnemyInfoData.StatusValue.isHitImmunity)
            {
                statusText = "피격이상 면역";

                if (currentEnemyInfoData.StatusValue.isSuperArmor)
                {
                    statusText += " / ";
                    statusText += "상태이상 면역";
                }
            }

            else if (currentEnemyInfoData.StatusValue.isSuperArmor)
            {
                statusText = "상태이상 면역";
            }

            if (currentEnemyInfoData.StatusValue.isInvincible)
                statusText = "모든 면역";

            return statusText;
        }

        private void HandleDeadEvent()
        {
            if (currentEnemyInfoData != null)
                currentEnemyInfoData.EnemyHpValue.OnHpChanged -= HandleHPChangeEvent;
            AllDisable();
            currentEnemyInfoData = null;
        }

        private void HandleHPChangeEvent(int current, int max)
        {
            Debug.Log($"HP Change: {current}/{max}");
            hpField.HpChange(current, max);
        }

        public void AllEnable()
        {
            string statusText = GetLmmunityIfo();
            gameObject.SetActive(true);
            hpField.EnableFor(currentEnemyInfoData.EnemyHpValue.CurrentHp, currentEnemyInfoData.EnemyHpValue.MaxHp);
            nameField.EnableFor(currentEnemyInfoData.Name);
            lmmunityInformationField.EnableFor(statusText);
        }

        public void AllDisable()
        {
            hpField.Disable();
            gameObject.SetActive(false);
        }
    }
}