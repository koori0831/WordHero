using System;
using System.Collections;
using UnityEngine;

namespace Work.Stages.Code
{
    
    public abstract class DoorEffect : MonoBehaviour
    {
        public abstract void Close();
        public abstract void Open();
    }
}