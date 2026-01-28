using UnityEngine;

namespace GondrLib.ObjectPool.RunTime
{
    public class PoolableEffect : MonoBehaviour, IPoolable
    {
        public PoolItemSO PoolItem { get; set; }

        public GameObject GameObject => gameObject;

        public void ResetItem()
        {
        }

        public void SetUpPool(Pool pool)
        {
        }
    }
}