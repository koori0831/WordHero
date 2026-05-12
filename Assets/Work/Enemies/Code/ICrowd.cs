using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Work.Enemies.Code
{
    public interface ICrowd
    {
        public float NeighborRadius { get; }
        public Transform Transform { get; }
    }
}
