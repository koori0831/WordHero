using UnityEngine;

namespace Work.Combat.Code
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        private int _damage;
        private float _speed;
        private GameObject _source;

        private const float LIFE_TIME = 15;
        private float _timer;

        public void Init(int damage, float speed, Vector3 forward, GameObject source = null)
        {
            _damage = damage;
            _speed = speed;
            transform.forward = forward;
            _source = source;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer > LIFE_TIME)
                Destroy(gameObject);

            transform.position += transform.forward * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if ((1 << collision.gameObject.layer & collisionLayer) != 0)
            {
                Debug.Log(collision.gameObject.name);

                DamageContext context = new DamageContext(_source != null ? _source : gameObject, collision.gameObject, _damage);
                DamageResolver.TryApplyDamage(context);

                if (collision.gameObject.TryGetComponent(out IKnockbackable knockbackable))
                {
                    knockbackable.TakeKnockback(new KnockbackData(5f, 0.5f, (knockbackable.Transform.position - transform.position).normalized, AnimationCurve.EaseInOut(0, 1, 1, 0))); ;
                }

                Destroy(gameObject);
            }
        }

    }
}
