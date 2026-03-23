using UnityEngine;

namespace Work.Weapons.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseProjectileMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float Speed = 15f;
        public float MaxLifeTime = 5f;

        protected Rigidbody _rb;

        public virtual void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            Destroy(gameObject, MaxLifeTime);
        }
    }
}