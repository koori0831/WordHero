using System.Collections;
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

        private HpBarInfoData _currentTargetInfoData;
        private Enemy _currentTargetEnemy;
        private bool _isNotDisabling = false;

        public void Awake()
        {
            hpField.Disable();
            Bus<InfoDataEvent>.Events += OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events += OnHideInfoDataEvent;
            Bus<EnemyHitEvent>.Events += OnEnemyHitEvent;
        }

        private void OnDestroy()
        {
            ResetEvents();
            Bus<InfoDataEvent>.Events -= OnInfoDataEvent;
            Bus<HideInfoDataEvent>.Events -= OnHideInfoDataEvent;
            Bus<EnemyHitEvent>.Events -= OnEnemyHitEvent;
        }

        private void OnHideInfoDataEvent(HideInfoDataEvent evt)
        {
            if (_currentTargetEnemy != null || _isNotDisabling)
                return;
            ResetEvents();
            AllDisable();
            _currentTargetInfoData = null;
            _currentTargetEnemy = null;
        }

        private void OnInfoDataEvent(InfoDataEvent evt)
        {
            if (_currentTargetInfoData != null) return;
            if (_currentTargetEnemy != null) return;
            if (!(evt.Info is HpBarInfoData data)) return;
            EnableFromInfo(data);
        }

        private void EnableFromInfo(HpBarInfoData data)
        {
            ResetEvents();
            _currentTargetInfoData = data;
            _currentTargetInfoData.HpValue.OnDead += HandleEnemyDeathEvent;
            _currentTargetInfoData.HpValue.OnHpChanged += HandleHPChangeEvent;
            _currentTargetInfoData.StatusValue.OnstateusChangeEvent += HandleStatusChangeEvent;
            AllEnable();
        }

        private void ResetEvents()
        {
            if (_currentTargetInfoData != null)
            {
                _currentTargetInfoData.HpValue.OnDead -= HandleEnemyDeathEvent;
                _currentTargetInfoData.HpValue.OnHpChanged -= HandleHPChangeEvent;
                _currentTargetInfoData.StatusValue.OnstateusChangeEvent -= HandleStatusChangeEvent;
            }
        }

        private void OnEnemyHitEvent(EnemyHitEvent evt)
        {
            Enemy hitTarget = evt.Target.GetComponent<Enemy>();
            if (hitTarget == null || (_currentTargetEnemy != null && hitTarget == _currentTargetEnemy)) return;
            _currentTargetEnemy = hitTarget;
            if (!(evt.Info is HpBarInfoData data)) return;
            EnableFromInfo(data);
        }

        private void HandleEnemyDeathEvent()
        {
            ResetEvents();
            _currentTargetEnemy = null;
            _currentTargetInfoData = null;

            if (!gameObject.activeInHierarchy)
            {
                _isNotDisabling = false;
                return;
            }

            _isNotDisabling = true;
            StartCoroutine(DelayDisable());
        }

        private IEnumerator DelayDisable()
        {
            yield return new WaitForSeconds(3f);
            if (gameObject.activeInHierarchy)
                AllDisable();

            _isNotDisabling = false;
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

            if (_currentTargetInfoData.StatusValue.isHitImmunity)
            {
                statusText = "피격이상 면역";

                if (_currentTargetInfoData.StatusValue.isSuperArmor)
                {
                    statusText += " / ";
                    statusText += "상태이상 면역";
                }
            }

            else if (_currentTargetInfoData.StatusValue.isSuperArmor)
            {
                statusText = "상태이상 면역";
            }

            if (_currentTargetInfoData.StatusValue.isInvincible)
                statusText = "모든 면역";

            return statusText;
        }

        private void HandleHPChangeEvent(int current, int max)
        {
            hpField.HpChange(current, max);
        }

        public void AllEnable()
        {
            string statusText = GetLmmunityIfo();
            gameObject.SetActive(true);
            hpField.EnableFor(_currentTargetInfoData.HpValue.CurrentHp, _currentTargetInfoData.HpValue.MaxHp);
            nameField.EnableFor(_currentTargetInfoData.Name);
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
