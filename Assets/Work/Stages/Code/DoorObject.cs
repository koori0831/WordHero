using System.Collections.Generic;
using UnityEngine;

namespace Work.Stages.Code
{
    public class DoorObject : MonoBehaviour
    {
        [SerializeField] public List<DoorEffect> doorEffects = new List<DoorEffect>();

        public void Open()
        {
            foreach (var effect in doorEffects)
            {
                effect.Open();
            }
        }

        public void Close()
        {
            foreach (var effect in doorEffects)
            {
                effect.Close();
            }
        }
    }
}