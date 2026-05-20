using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Work.Agents.Code;
using Work.Combat.Code;

namespace Work.Enemies.Code.AttackModules
{
    public class SpearComboAttackModule : EnemyAttackModule
    {
        [Header("Spear Hit Box")]
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Vector3 hitBoxHalfExtents = new Vector3(0.4f, 0.8f, 1.4f);
        [SerializeField] private float hitBoxForwardOffset = 1.4f;
        [SerializeField] private float hitBoxHeightOffset = 0.8f;

        [Header("Combo")]
        [SerializeField] private int maxStabCount = 3;
        [SerializeField] private float stabAdvanceDistance = 0.35f;
        [SerializeField] private float stabAdvanceDuration = 0.08f;

        [Header("Knockback")]
        [SerializeField] private float knockbackForce = 2f;
        [SerializeField] private float knockbackDuration = 0.15f;
        [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Effects")]
        [SerializeField] private Transform spearTip;
        [SerializeField] private GameObject prepareEffectPrefab;
        [SerializeField] private GameObject stabEffectPrefab;
        [SerializeField] private float effectDestroyDelay = 1.5f;

        [Header("Warning Decal")]
        [SerializeField] private AttackWarningDecal warningDecalPrefab;
        [SerializeField] private float warningDecalProjectionHeight = 4f;
        [SerializeField] private float warningDecalFadeInDuration = 0.12f;
        [SerializeField] private float warningDecalFadeOutDuration = 0.08f;
        [SerializeField] private float warningDecalHeightOffset = 2f;

        private int stabIndex;
        private Coroutine advanceCoroutine;
        private AttackWarningDecal activeWarningDecal;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            stabIndex = 0;
            if (_owner != null)
            {
                _owner.OnHitEvent.AddListener(HandleOwnerHit);
            }
        }

        public void PlayPrepareEffect()
        {
            SpawnEffect(prepareEffectPrefab, GetEffectPosition(), _owner.transform.rotation);
        }

        public void ResetCombo()
        {
            stabIndex = 0;
            HideWarningDecal();
        }

        public void ShowWarningDecal()
        {
            if (_owner == null || warningDecalPrefab == null)
                return;

            if (activeWarningDecal == null)
            {
                activeWarningDecal = Instantiate(warningDecalPrefab);
            }

            activeWarningDecal.ShowFilled(
                GetWarningDecalPosition(),
                GetWarningDecalRotation(),
                GetWarningDecalSize(),
                warningDecalProjectionHeight,
                _owner.transform.forward,
                warningDecalFadeInDuration);
        }

        public void HideWarningDecal()
        {
            if (activeWarningDecal == null)
                return;

            activeWarningDecal.Hide(warningDecalFadeOutDuration);
            activeWarningDecal = null;
        }

        public void HideWarningDecalImmediate()
        {
            if (activeWarningDecal == null)
                return;

            activeWarningDecal.HideImmediate();
            activeWarningDecal = null;
        }

        public override void Attack()
        {
            if (_owner == null)
                return;

            if (stabIndex >= maxStabCount)
                return;

            HideWarningDecal();
            stabIndex++;
            PlayStabEffect();
            ApplySpearDamage();
            StartStabAdvance();
        }

        private void Update()
        {
            if (_owner != null && _owner.IsDead)
            {
                HideWarningDecalImmediate();
            }
        }

        private void PlayStabEffect()
        {
            SpawnEffect(stabEffectPrefab, GetEffectPosition(), _owner.transform.rotation);
        }

        private void ApplySpearDamage()
        {
            Collider[] hits = Physics.OverlapBox(GetHitBoxCenter(), hitBoxHalfExtents, _owner.transform.rotation, targetLayer);
            HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

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
            }
        }

        private void StartStabAdvance()
        {
            if (stabAdvanceDistance <= 0f || stabAdvanceDuration <= 0f)
                return;

            if (advanceCoroutine != null)
            {
                StopCoroutine(advanceCoroutine);
            }

            advanceCoroutine = StartCoroutine(StabAdvance());
        }

        private IEnumerator StabAdvance()
        {
            float elapsed = 0f;
            Vector3 direction = _owner.transform.forward;

            while (elapsed < stabAdvanceDuration)
            {
                float delta = Mathf.Min(Time.deltaTime, stabAdvanceDuration - elapsed);
                Vector3 movement = direction * (stabAdvanceDistance * (delta / stabAdvanceDuration));

                if (_owner.NavAgent != null && _owner.NavAgent.enabled)
                {
                    _owner.NavAgent.Move(movement);
                }
                else
                {
                    _owner.transform.position += movement;
                }

                elapsed += delta;
                yield return null;
            }

            advanceCoroutine = null;
        }

        private Vector3 GetHitBoxCenter()
        {
            return _owner.transform.position
                + Vector3.up * hitBoxHeightOffset
                + _owner.transform.forward * hitBoxForwardOffset;
        }

        private Vector3 GetEffectPosition()
        {
            return spearTip != null ? spearTip.position : GetHitBoxCenter();
        }

        private Vector3 GetWarningDecalPosition()
        {
            Vector3 hitBoxCenter = GetHitBoxCenter();
            return new Vector3(hitBoxCenter.x, _owner.transform.position.y + warningDecalHeightOffset, hitBoxCenter.z);
        }

        private Quaternion GetWarningDecalRotation()
        {
            return Quaternion.LookRotation(Vector3.down, _owner.transform.forward);
        }

        private Vector2 GetWarningDecalSize()
        {
            return new Vector2(hitBoxHalfExtents.x * 2f, hitBoxHalfExtents.z * 2f);
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
            HideWarningDecalImmediate();
        }

        private void OnDisable()
        {
            HideWarningDecalImmediate();
        }

        private void OnDestroy()
        {
            if (_owner != null)
            {
                _owner.OnHitEvent.RemoveListener(HandleOwnerHit);
            }

            HideWarningDecalImmediate();
        }

        private void OnDrawGizmosSelected()
        {
            Transform ownerTransform = transform;
            Gizmos.color = Color.cyan;
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Vector3 center = ownerTransform.position
                + Vector3.up * hitBoxHeightOffset
                + ownerTransform.forward * hitBoxForwardOffset;

            Gizmos.matrix = Matrix4x4.TRS(center, ownerTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, hitBoxHalfExtents * 2f);
            Gizmos.matrix = previousMatrix;
        }
    }
}
