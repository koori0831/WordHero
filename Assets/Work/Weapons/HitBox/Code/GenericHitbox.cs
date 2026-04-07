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

        [SerializeReference]
        public List<IHitEffect> OnHitEffects = new List<IHitEffect>();

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