using UnityEngine;
using Work.Combat.Code;
using Work.Core.Utils.EventBus;
using Work.Cursor.Code;
using Work.Enemies.Code;
using Work.Information.Code;

namespace Work.HPBar.Code
{
    //
    public class EnemyHpPanel : MonoBehaviour
    {
        [SerializeField] private HPField hpField;
        [SerializeField] private NameField nameField;
        [SerializeField] private LmmunityInformationField lmmunityInformationField;

        private EnemyInfoDataSO _currentEnemyInfoData;
        private Enemy _currentTargetEnemy;

        public void Awake()
        {
            hpField.Disable();
            Bus<InfoDataEvent>.Events += OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events += OnHideInfoDataEvent;
            Bus<EnemyHitEvent>.Events += OnEnemyHitEvent;
        }

        private void OnDestroy()
        {
            Bus<InfoDataEvent>.Events -= OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events -= OnHideInfoDataEvent;
            Bus<EnemyHitEvent>.Events -= OnEnemyHitEvent;
        }

        private void OnHideInfoDataEvent(HideInfoDataEvent evt)
        {
            if (_currentTargetEnemy != null)
                return;
            ResetEvents();
            AllDisable();
            _currentEnemyInfoData = null;
            _currentTargetEnemy = null;
        }

        private void OnInfoDataEvent(InfoDataEvent evt)
        {
            if (_currentEnemyInfoData != null) return;
            if (_currentTargetEnemy != null) return;
            if (!(evt.Info is EnemyInfoDataSO data)) return;
            EnableFromInfo(data);
        }

        private void EnableFromInfo(EnemyInfoDataSO data)
        {
            ResetEvents();
            _currentEnemyInfoData = data;
            _currentEnemyInfoData.EnemyHpValue.OnDead += HandleEnemyDeathEvent;
            _currentEnemyInfoData.EnemyHpValue.OnHpChanged += HandleHPChangeEvent;
            _currentEnemyInfoData.StatusValue.OnstateusChangeEvent += HandleStatusChangeEvent;
            AllEnable();
        }

        private void ResetEvents()
        {
            if (_currentEnemyInfoData != null)
            {
                _currentEnemyInfoData.EnemyHpValue.OnHpChanged -= HandleHPChangeEvent;
                _currentEnemyInfoData.StatusValue.OnstateusChangeEvent -= HandleStatusChangeEvent;
            }
        }

        private void OnEnemyHitEvent(EnemyHitEvent evt)
        {
            Enemy hitTarget = evt.Target.GetComponent<Enemy>();
            if (hitTarget == null || (_currentTargetEnemy != null && hitTarget == _currentTargetEnemy)) return;
            _currentTargetEnemy = hitTarget;
            if (!(evt.Info is EnemyInfoDataSO data)) return;
            data.EnemyHpValue.OnDead += HandleEnemyDeathEvent;
            EnableFromInfo(data);
        }

        private void HandleEnemyDeathEvent()
        {
            ResetEvents();
            AllDisable();
            _currentTargetEnemy = null;
            _currentEnemyInfoData = null;
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

            if (_currentEnemyInfoData.StatusValue.isHitImmunity)
            {
                statusText = "피격이상 면역";

                if (_currentEnemyInfoData.StatusValue.isSuperArmor)
                {
                    statusText += " / ";
                    statusText += "상태이상 면역";
                }
            }

            else if (_currentEnemyInfoData.StatusValue.isSuperArmor)
            {
                statusText = "상태이상 면역";
            }

            if (_currentEnemyInfoData.StatusValue.isInvincible)
                statusText = "모든 면역";

            return statusText;
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
            hpField.EnableFor(_currentEnemyInfoData.EnemyHpValue.CurrentHp, _currentEnemyInfoData.EnemyHpValue.MaxHp);
            nameField.EnableFor(_currentEnemyInfoData.Name);
            lmmunityInformationField.EnableFor(statusText);
        }

        public void AllDisable()
        {
            hpField.Disable();
            nameField.Disable();
            lmmunityInformationField.Disable();
            gameObject.SetActive(false);
        }
    }
}