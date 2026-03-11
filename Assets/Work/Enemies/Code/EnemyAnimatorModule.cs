using UnityEngine;
using Work.Agents.Code;

namespace Work.Enemies.Code
{
    public class EnemyAnimatorModule : MonoBehaviour, IAgentModule
    {
        private Animator _animator;
        public Animator Animator => _animator;
        private Enemy _owner;

        public Renderer[] Renderers { get; private set; }

        public void Initialize(Agent agent)
        {
            _owner = agent as Enemy;
            _animator = GetComponent<Animator>();
            Renderers = GetComponentsInChildren<Renderer>();
        }

        public void SetParam(int animHash , bool value) => _animator.SetBool(animHash, value);
        public void SetParam(int animHash , int value) => _animator.SetInteger(animHash, value);
        public void SetParam(int animHash , float value) => _animator.SetFloat(animHash, value);
        
    }
}