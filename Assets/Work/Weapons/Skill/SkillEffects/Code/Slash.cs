using UnityEngine;
using Work.Combat.Code;

namespace Work.Weapons.Skill.SkillEffects.Code
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Slash : MonoBehaviour
    {
        [field: SerializeField] public int Damage { get; set; }
        [SerializeField] private float speed = 10f;
        [SerializeField] private GameObject hitEffectPrefab;

        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();

            _rb.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            DamageContext context = new DamageContext(gameObject, other.gameObject, Damage);
            KnockbackData knockbackData = new KnockbackData(speed * 0.3f, 0.5f, transform.forward, AnimationCurve.EaseInOut(0, 1, 1, 0));
            DamageResolver.TryApplyDamage(context, true, knockbackData);

            if (hitEffectPrefab != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
            }
        }

        private void FixedUpdate()
        {
            _rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
        }
    }
}