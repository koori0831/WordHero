using Code.Entities;
using Code.FSM;
using Work.Entities;
using UnityEngine;
using Work.StatSystem.Code;
using Work.Combat.Code;
using Work.Core.Utils.EventBus;
using Work.Enemies.Code;
using UnityEngine.InputSystem;
using Work.Input.Code;

namespace Work.Player.Code
{
    public class Player : Entity, IDamageable
    {
        [SerializeField] private BoxCollider _attackCollider;
        [SerializeField] private AnimationCurve _attackKnockbackCurve;
        [SerializeField] private float _lockOnDetectRange = 5f;
        [SerializeField] private LayerMask _lockOnTargetLayer = ~0;

        private EntityHealth _health;
        private StateMachine _stateMachine;
        private EntityStatCompo _stat;
        private StatSO _attackSO;
        [SerializeField, Range(0f, 1f)] private float _criticalChance = 0f;

        private PlayerInputRoot _inputRoot;

        private void OnEnable()
        {
            _health = GetCompo<EntityHealth>();
            _stateMachine = GetCompo<StateCompo>().StateMachine;
            _stat = GetCompo<EntityStatCompo>();
            _inputRoot = GetCompo<PlayerInputRoot>();

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
            if (_inputRoot.Input.CurrentDeviceType == InputDeviceType.KeyboardMouse)
                RotateToMousePosition();
            else
                RotateToNearestEnemy();

            // 어택 콜라이더 영역에 있는 IDamageable 오브젝트에 데미지 적용
            Collider[] hitColliders = Physics.OverlapBox(_attackCollider.bounds.center, _attackCollider.bounds.extents, _attackCollider.transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == this.gameObject) continue; // 자기 자신은 무시
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = (int)_attackSO.Value;
                    bool isCritical = Random.value <= _criticalChance;
                    damageable.TakeDamage(damage);

                    Component targetComponent = damageable as Component;
                    GameObject targetObject = targetComponent != null ? targetComponent.gameObject : null;
                    Bus<CombatHitEvent>.Raise(new CombatHitEvent(gameObject, targetObject, damage, isCritical));
                }

                IKnockbackable knockbackable = hitCollider.GetComponent<IKnockbackable>();
                if (knockbackable != null)
                {
                    Vector3 knockbackDirection = (hitCollider.transform.position - transform.position).normalized;
                    KnockbackData knockbackData = new KnockbackData(7f, 0.15f, knockbackDirection, _attackKnockbackCurve);
                    knockbackable.TakeKnockback(knockbackData);
                }
            }
        }

        private void RotateToMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, _lockOnTargetLayer))
            {
                Vector3 lookDirection = hitInfo.point - transform.position;
                lookDirection.y = 0f; // 수평 회전만 허용
                if (lookDirection.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
                }
            }
        }

        private void RotateToNearestEnemy()
        {
            Collider[] detected = Physics.OverlapSphere(transform.position, _lockOnDetectRange, _lockOnTargetLayer);
            Enemy nearestEnemy = null;
            float nearestSqrDistance = float.MaxValue;

            foreach (Collider col in detected)
            {
                if (col == null || col.gameObject == gameObject)
                    continue;

                Enemy enemy = col.GetComponentInParent<Enemy>();
                if (enemy == null)
                    continue;

                Vector3 toEnemy = enemy.transform.position - transform.position;
                toEnemy.y = 0f;
                float sqrDistance = toEnemy.sqrMagnitude;

                if (sqrDistance > 0.0001f && sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy == null)
                return;

            Vector3 lookDirection = nearestEnemy.transform.position - transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude <= 0.0001f)
                return;

            transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _lockOnDetectRange);
        }
    }
}
