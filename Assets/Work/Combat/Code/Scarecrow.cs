using UnityEngine;
using UnityEngine.Events;
using Work.Agents.Code;
using Work.Information.Code;

namespace Work.Combat.Code
{
    public class Scarecrow : MonoBehaviour, IDamageable, ISelectable
    {
        [SerializeField] private Animator animator;
        private int animHash => Animator.StringToHash("Hit");

        public bool IsCanShowInfo => true;

        public InfoDataSO InfoData => _scarecrowInfo;
        private ScarecrowInfoDataSO _scarecrowInfo;
        [SerializeField] private ScarecrowInfoDataSO scarecrowInfoPrefab;

        private int _maxHealth = 10000;
        public int CurrentHealth
        {
            get => _hpValue.CurrentHp;
            private set
            {
                _hpValue.Update(value);
            }
        }

        private HpValue _hpValue;
        private StatusValue _statusValue;

        public UnityEvent OnHitEvent;

        private void Awake()
        {
            _hpValue = new HpValue(_maxHealth);
            _statusValue = new StatusValue();

            _scarecrowInfo = scarecrowInfoPrefab.GetInfo(_hpValue, _statusValue) as ScarecrowInfoDataSO;
        }

        public void TakeDamage(int damageAmount)
        {
            CurrentHealth -= damageAmount;
            if (CurrentHealth <= 0) { CurrentHealth = _maxHealth; }
            _hpValue.OnHpChanged?.Invoke(CurrentHealth,_maxHealth );
            animator.SetBool(animHash, true);
            OnHitEvent?.Invoke();
            SetFalseAnim();
        }

        private async void SetFalseAnim()
        {
            await Awaitable.NextFrameAsync();
            animator.SetBool(animHash, false);
        }
    }
}