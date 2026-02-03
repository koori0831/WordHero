using UnityEngine;

namespace Code.FSM
{
    [CreateAssetMenu(fileName = "StateData", menuName = "FSM/StateData")]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string targetClass;
        public string statePath;

        [SerializeField, HideInInspector] private int _animationHash;

        public int animationHash
        {
            get
            {
                if (_animationHash == 0 && !string.IsNullOrEmpty(stateName))
                {
                    _animationHash = Animator.StringToHash(stateName);
                }
                return _animationHash;
            }
        }

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(stateName))
                _animationHash = Animator.StringToHash(stateName);
        }
    }
}