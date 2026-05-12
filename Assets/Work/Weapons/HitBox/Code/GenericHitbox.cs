using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Weapons.Imprint.Code;

namespace Work.Weapons.HitBox.Code
{
    [RequireComponent(typeof(Collider))]
    public class GenericHitbox : MonoBehaviour
    {
        public GameObject Owner { get; set; }

        public LayerMask TargetLayer;
        public bool DestroyOnHit = true;
        public bool DestroyOnStart = true;
        [ShowIf(nameof(DestroyOnStart))]
        public float DestroyDelay = 5f;
        [ShowIf(nameof(DestroyOnStart))]
        public bool DestroyParent = true;

        [SerializeReference]
        public List<IHitEffect> OnHitEffects = new List<IHitEffect>();

        private void Start()
        {
            if (DestroyOnStart)
            {
                if (DestroyParent && gameObject.transform.parent != null)
                {
                    Destroy(gameObject.transform.parent.gameObject, DestroyDelay);
                }
                else
                {
                    Destroy(gameObject, DestroyDelay);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Owner != null && other.gameObject == Owner) return;
            if ((TargetLayer.value & (1 << other.gameObject.layer)) == 0) return;

            Bus<OnHitSuccessTrigger>.Raise(new OnHitSuccessTrigger());

            Vector3 hitPoint = other.ClosestPoint(transform.position);

            foreach (var effect in OnHitEffects)
            {
                effect.ExecuteHit(Owner != null ? Owner : gameObject, other.gameObject, hitPoint);
            }

            if (DestroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}