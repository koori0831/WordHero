using UnityEngine;

namespace Code.FSM
{
    [CreateAssetMenu(fileName = "StateData", menuName = "FSM/StateData")]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string targetClass;
        public int animationHash { get; private set; }

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(stateName))
                animationHash = Animator.StringToHash(stateName);
        }
    }
}