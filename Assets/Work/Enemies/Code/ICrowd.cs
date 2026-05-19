using UnityEngine;

namespace Work.Enemies.Code
{
    public interface ICrowd
    {
        public float NeighborRadius { get; }
        public Transform Transform { get; }
    }
}
