using System.Collections;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Enemies.Code.AttackModules
{
    public class ChargeAttackModule : EnemyAttackModule
    {
        private enum ChargeResult
        {
            None,
            HitTarget,
            HitObstacle,
            MissedClose,
            MissedFar
        }

        [Header("Charge")]
        [SerializeField] private bool useSeparatedAnimationEvents = true;
        [SerializeField] private float chargeSpeed = 26f;
        [SerializeField] private float chargeDuration = 0.28f;
        [SerializeField] private float chargeDistance = 6.5f;
        [SerializeField] private float chargeWarningDuration = 0.65f;
        [SerializeField] private float chargeRecoveryDuration = 0.15f;

        [Header("Decision")]
        [SerializeField] private float meleeTransitionDistance = 2.5f;
        [SerializeField] private float navMeshRecoverSampleRadius = 1.5f;

        [Header("Charge Hit Box")]
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private Vector3 chargeHitBoxHalfExtents = new Vector3(0.75f, 1f, 0.9f);
        [SerializeField] private float chargeHitBoxForwardOffset = 0.9f;
        [SerializeField] private float chargeHitBoxHeightOffset = 1f;

        [Header("Melee Hit Box")]
        [SerializeField] private Vector3 meleeHitBoxHalfExtents = new Vector3(0.8f, 1f, 0.9f);
        [SerializeField] private float meleeHitBoxForwardOffset = 1f;
        [SerializeField] private float meleeHitBoxHeightOffset = 1f;

        [Header("Warning Decal")]
        [SerializeField] private AttackWarningDecal warningDecalPrefab;
        [SerializeField] private float warningDecalProjectionHeight = 4f;
        [SerializeField] private float warningDecalHeightOffset = 2f;
        [SerializeField] private float warningDecalWidth = 1.5f;

        [Header("Knockback")]
        [SerializeField] private float knockbackForce = 3f;
        [SerializeField] private float knockbackDuration = 0.18f;
        [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Effects")]
        [SerializeField] private GameObject chargeStartEffectPrefab;
        [SerializeField] private GameObject chargeImpactEffectPrefab;
        [SerializeField] private float effectDestroyDelay = 1.5f;

        private readonly HashSet<GameObject> damagedTargets = new HashSet<GameObject>();
        private EnemyMovementModule movementModule;
        private Transform target;
        private Vector3 chargeDirection;
        private Coroutine chargeCoroutine;
        private Coroutine recoveryCoroutine;
        private AttackWarningDecal activeWarningDecal;
        private ChargeResult lastChargeResult;
        private bool isCharging;
        private bool hasLockedChargeDirection;

        public bool IsTargetInMeleeTransitionDistance => GetDistanceToTarget() <= meleeTransitionDistance;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            movementModule = _owner.GetModule<EnemyMovementModule>(true);
            chargeDirection = transform.forward;

            if (_owner != null)
            {
                _owner.OnHitEvent.AddListener(HandleOwnerHit);
            }
        }

        public override void BTInit()
        {
            base.BTInit();

            BlackboardVariable<Transform> targetVariable = _owner.GetBlackboardVariable<Transform>(BTVariables.Target);
            if (targetVariable != null)
            {
                target = targetVariable.Value;
            }
        }

        public override void Attack()
        {
            if (useSeparatedAnimationEvents)
                return;

            StartCharge();
        }

        public void ShowChargeWarningDecal()
        {
            if (_owner == null || warningDecalPrefab == null)
                return;

            CacheChargeDirection();
            hasLockedChargeDirection = true;
            HideWarningDecalImmediate();

            activeWarningDecal = Instantiate(warningDecalPrefab);
            activeWarningDecal.ShowFilled(
                GetWarningDecalCenter(),
                GetWarningDecalRotation(),
                new Vector2(warningDecalWidth, chargeDistance),
                warningDecalProjectionHeight,
                chargeDirection,
                chargeWarningDuration);
        }

        public void StartCharge()
        {
            if (_owner == null || _owner.IsDead)
                return;

            if (isCharging)
                return;

            HideWarningDecalImmediate();
            if (hasLockedChargeDirection == false)
            {
                CacheChargeDirection();
            }

            StopRunningCoroutines();
            damagedTargets.Clear();
            lastChargeResult = ChargeResult.None;
            isCharging = true;

            if (movementModule != null)
            {
                movementModule.SetAutoMove(false);
                movementModule.SetMovement(false);
                SetNavStop(true);
            }

            SpawnEffect(chargeStartEffectPrefab, GetChargeHitBoxCenter(), _owner.transform.rotation);
            chargeCoroutine = StartCoroutine(ChargeRoutine());
        }

        public void EndCharge()
        {
            if (isCharging)
            {
                StopCharge(ResolveMissResult());
            }
        }

        public void ResetCharge()
        {
            StopRunningCoroutines();
            HideWarningDecalImmediate();
            damagedTargets.Clear();
            isCharging = false;
            lastChargeResult = ChargeResult.None;
            hasLockedChargeDirection = false;
            RestoreMovement();
        }

        public void MeleeAttack()
        {
            if (_owner == null || _owner.IsDead)
                return;

            damagedTargets.Clear();
            ApplyDamage(GetMeleeHitBoxCenter(), meleeHitBoxHalfExtents);
        }

        private IEnumerator ChargeRoutine()
        {
            float elapsed = 0f;

            while (elapsed < chargeDuration)
            {
                if (_owner == null || _owner.IsDead)
                {
                    ResetCharge();
                    yield break;
                }

                float delta = Mathf.Min(Time.deltaTime, chargeDuration - elapsed);
                float moveDistance = chargeSpeed * delta;

                if (IsObstacleAhead(moveDistance))
                {
                    StopCharge(ChargeResult.HitObstacle);
                    yield break;
                }

                MoveCharge(moveDistance);

                if (TryApplyChargeDamage())
                {
                    StopCharge(ChargeResult.HitTarget);
                    yield break;
                }

                elapsed += delta;
                yield return null;
            }

            StopCharge(ResolveMissResult());
        }

        private void StopCharge(ChargeResult result)
        {
            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
            }

            HideWarningDecalImmediate();
            isCharging = false;
            lastChargeResult = result;
            SpawnEffect(chargeImpactEffectPrefab, GetChargeHitBoxCenter(), _owner.transform.rotation);

            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
            }

            recoveryCoroutine = StartCoroutine(RecoveryRoutine());
        }

        private IEnumerator RecoveryRoutine()
        {
            if (chargeRecoveryDuration > 0f)
            {
                yield return new WaitForSeconds(chargeRecoveryDuration);
            }

            recoveryCoroutine = null;

            if (useSeparatedAnimationEvents)
            {
                RestoreMovement();
                yield break;
            }

            RestoreMovement();
        }

        private ChargeResult ResolveMissResult()
        {
            return GetDistanceToTarget() <= meleeTransitionDistance ? ChargeResult.MissedClose : ChargeResult.MissedFar;
        }

        private bool TryApplyChargeDamage()
        {
            return ApplyDamage(GetChargeHitBoxCenter(), chargeHitBoxHalfExtents);
        }

        private bool ApplyDamage(Vector3 center, Vector3 halfExtents)
        {
            Collider[] hits = Physics.OverlapBox(center, halfExtents, _owner.transform.rotation, targetLayer);
            bool appliedDamage = false;

            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable damageable = hits[i].GetComponentInParent<IDamageable>();
                if (damageable == null)
                    continue;

                Component damageableComponent = damageable as Component;
                GameObject targetObject = damageableComponent != null ? damageableComponent.gameObject : hits[i].gameObject;
                if (damagedTargets.Add(targetObject) == false)
                    continue;

                Vector3 direction = (targetObject.transform.position - _owner.transform.position).normalized;
                KnockbackData knockbackData = new KnockbackData(knockbackForce, knockbackDuration, direction, knockbackCurve);
                DamageContext context = new DamageContext(_owner.gameObject, targetObject, damage);
                DamageResolver.TryApplyDamage(context, true, knockbackData);
                appliedDamage = true;
            }

            return appliedDamage;
        }

        private bool IsObstacleAhead(float distance)
        {
            if (obstacleLayer.value == 0)
                return false;

            return Physics.BoxCast(
                GetChargeHitBoxCenter(),
                chargeHitBoxHalfExtents,
                chargeDirection,
                _owner.transform.rotation,
                distance,
                obstacleLayer,
                QueryTriggerInteraction.Ignore);
        }

        private void MoveCharge(float distance)
        {
            Vector3 movement = chargeDirection * distance;

            if (_owner.NavAgent != null && _owner.NavAgent.enabled && _owner.NavAgent.isOnNavMesh)
            {
                _owner.NavAgent.Move(movement);
            }
            else
            {
                _owner.transform.position += movement;
            }
        }

        private void CacheChargeDirection()
        {
            Vector3 direction = target != null
                ? target.position - _owner.transform.position
                : _owner.transform.forward;

            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = _owner.transform.forward;
            }

            chargeDirection = direction.normalized;
            _owner.transform.rotation = Quaternion.LookRotation(chargeDirection);
        }

        private Vector3 GetChargeHitBoxCenter()
        {
            return _owner.transform.position
                + Vector3.up * chargeHitBoxHeightOffset
                + chargeDirection * chargeHitBoxForwardOffset;
        }

        private Vector3 GetMeleeHitBoxCenter()
        {
            return _owner.transform.position
                + Vector3.up * meleeHitBoxHeightOffset
                + _owner.transform.forward * meleeHitBoxForwardOffset;
        }

        private Vector3 GetWarningDecalCenter()
        {
            Vector3 center = _owner.transform.position + chargeDirection * (chargeDistance * 0.5f);
            return new Vector3(center.x, _owner.transform.position.y + warningDecalHeightOffset, center.z);
        }

        private Quaternion GetWarningDecalRotation()
        {
            return Quaternion.LookRotation(Vector3.down, chargeDirection);
        }

        private float GetDistanceToTarget()
        {
            if (target == null)
                return float.MaxValue;

            Vector3 offset = target.position - _owner.transform.position;
            offset.y = 0f;
            return offset.magnitude;
        }

        private void RestoreMovement()
        {
            if (movementModule == null)
                return;

            RecoverToNavMesh();
            movementModule.SetMovement(true);
            SetNavStop(false);
        }

        private void SetNavStop(bool isStop)
        {
            if (_owner != null && _owner.NavAgent != null && _owner.NavAgent.enabled && _owner.NavAgent.isOnNavMesh)
            {
                movementModule.SetStop(isStop);
            }
        }

        private void RecoverToNavMesh()
        {
            if (_owner == null || _owner.NavAgent == null || _owner.NavAgent.enabled == false || _owner.NavAgent.isOnNavMesh)
                return;

            if (NavMesh.SamplePosition(_owner.transform.position, out NavMeshHit hit, navMeshRecoverSampleRadius, NavMesh.AllAreas))
            {
                _owner.NavAgent.Warp(hit.position);
            }
        }

        private void StopRunningCoroutines()
        {
            if (chargeCoroutine != null)
            {
                StopCoroutine(chargeCoroutine);
                chargeCoroutine = null;
            }

            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
                recoveryCoroutine = null;
            }
        }

        private void HideWarningDecalImmediate()
        {
            if (activeWarningDecal == null)
                return;

            activeWarningDecal.HideImmediate();
            activeWarningDecal = null;
        }

        private void SpawnEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation)
        {
            if (effectPrefab == null)
                return;

            GameObject effect = Instantiate(effectPrefab, position, rotation);
            if (effectDestroyDelay > 0f)
            {
                Destroy(effect, effectDestroyDelay);
            }
        }

        private void HandleOwnerHit(int damageAmount)
        {
            ResetCharge();
        }

        private void Update()
        {
            if (_owner != null && _owner.IsDead)
            {
                ResetCharge();
            }
        }

        private void OnDisable()
        {
            ResetCharge();
        }

        private void OnDestroy()
        {
            if (_owner != null)
            {
                _owner.OnHitEvent.RemoveListener(HandleOwnerHit);
            }

            ResetCharge();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Transform ownerTransform = transform;
            Matrix4x4 previousMatrix = Gizmos.matrix;

            Gizmos.color = Color.red;
            Vector3 chargeCenter = ownerTransform.position
                + Vector3.up * chargeHitBoxHeightOffset
                + ownerTransform.forward * chargeHitBoxForwardOffset;
            Gizmos.matrix = Matrix4x4.TRS(chargeCenter, ownerTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, chargeHitBoxHalfExtents * 2f);

            Gizmos.color = Color.yellow;
            Vector3 meleeCenter = ownerTransform.position
                + Vector3.up * meleeHitBoxHeightOffset
                + ownerTransform.forward * meleeHitBoxForwardOffset;
            Gizmos.matrix = Matrix4x4.TRS(meleeCenter, ownerTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, meleeHitBoxHalfExtents * 2f);

            Gizmos.matrix = previousMatrix;
        }
    }
}
