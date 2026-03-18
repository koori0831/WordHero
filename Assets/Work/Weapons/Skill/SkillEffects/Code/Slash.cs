using UnityEngine;
using Work.Combat.Code;
using Work.Enemies.Code;

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
            if (other.TryGetComponent(out Enemy damageable))
            {
                DamageResolver.TryApplyDamage(new DamageContext(gameObject, other.gameObject, Damage));

                if (hitEffectPrefab != null)
                {
                    Vector3 hitPoint = other.ClosestPoint(transform.position);
                    Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
                }
            }
        }

        private void Update()
        {
            _rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
        }
    }
}