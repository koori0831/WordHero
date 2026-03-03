using UnityEngine;
using Work.Entities;

namespace Work.Combat.Code
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private LayerMask collisionLayer;
        private int _damage;
        private float _speed;

        private const float LIFE_TIME = 15;
        private float _timer;

        public void Init(int damage, float speed, Vector3 forward)
        {
            _damage = damage;
            _speed = speed;
            transform.forward = forward;
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

                if (collision.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(_damage);
                }


                Destroy(gameObject);
            }
        }

    }
}