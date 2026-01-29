using System;
using TMPro;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyHPBarModule : MonoBehaviour, IEnemyModule
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private GameObject hpBarObject;
        [SerializeField] private HPBar bar;

        private Enemy _owner;
        private EnemyHealthModule _healthModule;

        private float _hpBarPercent;

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            _healthModule = _owner.GetModule<EnemyHealthModule>();

            _healthModule.OnHealthChanged.AddListener(UpdateHPBar);

            UpdateHPBar(_healthModule.MaxHealth, _healthModule.MaxHealth);
        }

        public void Update()
        {
            hpBarObject.transform.localScale = new Vector3(_hpBarPercent, 1f, 1f);
        }

        private void UpdateHPBar(int previorsValue, int changeValue)
        {
            int maxHealth = _healthModule.MaxHealth;
            int currentHealth = changeValue;

            _hpBarPercent = (float)currentHealth / maxHealth;
            _hpBarPercent = Mathf.Clamp01(_hpBarPercent);

            int cur = currentHealth < 0 ? 0 : currentHealth;
            text.SetText($"{cur}/{maxHealth}");
        }

        public void SetActiveBar(bool isTrue = true)
        {
            bar.SetActive(isTrue);
        }
    }
}