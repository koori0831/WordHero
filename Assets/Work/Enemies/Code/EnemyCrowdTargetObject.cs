using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyCrowdTargetObject : MonoBehaviour, ICrowd
    {
        public float NeighborRadius => 5;

        public Transform Transform => gameObject != null ? transform : null;
    }
}
