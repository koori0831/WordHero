using Code.Entities;
using Code.FSM;
using Work.Entities;
using UnityEngine;
using Work.StatSystem.Code;

namespace Work.Player.Code
{
    public class Player : Entity, IDamageable
    {
        [SerializeField] private BoxCollider _attackCollider;

        private EntityHealth _health;
        private StateMachine _stateMachine;
        private EntityStatCompo _stat;
        private StatSO _attackSO;

        private void OnEnable()
        {
            _health = GetCompo<EntityHealth>();
            _stateMachine = GetCompo<StateCompo>().StateMachine;
            _stat = GetCompo<EntityStatCompo>();

            _health.DeadTrigger += OnDead;
            _health.DamagedTrigger += OnHit;
        }

        private void Start()
        {
            _stat.TryGetStat("AttackPower", out _attackSO);
        }

        private void OnDisable()
        {
            _health.DeadTrigger -= OnDead;
            _health.DamagedTrigger -= OnHit;
        }

        private void OnDead() => _stateMachine.ChangeState("Death");
        private void OnHit() => _stateMachine.ChangeState("Hit");

        public void TakeDamage(int damageAmount)
        {
            _health.DecreaseHP(damageAmount);
        }

        public void Attack()
        {
            // 어택 콜라이더 영역에 있는 IDamageable 오브젝트에 데미지 적용
            Collider[] hitColliders = Physics.OverlapBox(_attackCollider.bounds.center, _attackCollider.bounds.extents, _attackCollider.transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == this.gameObject) continue; // 자기 자신은 무시
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = (int)_attackSO.Value;
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}