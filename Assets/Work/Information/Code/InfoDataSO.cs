using System;
using UnityEngine;

namespace Work.Information.Code
{
    public abstract class InfoDataSO : ScriptableObject, ICloneable
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public string Description { get; private set; }

        public object Clone()
        {
            return Instantiate(this);
        }

        public virtual InfoDataSO GetInfo()
        {
            InfoDataSO data = Clone() as InfoDataSO;
            return data;
        }
    }
}