using UnityEngine;

namespace Work.ETC.ColliderUtills
{
    [RequireComponent(typeof(Collider))]
    public class LifeTimeCollider : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 0.25f;

        public void Update()
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                GetComponent<Collider>().enabled = false;
                Destroy(this);
            }
        }
    }
}