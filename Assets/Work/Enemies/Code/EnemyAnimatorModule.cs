using UnityEngine;

namespace Work.Enemies.Code
{
    public class EnemyAnimatorModule : MonoBehaviour, IEnemyModule
    {
        private Animator _animator;
        public Animator Animator => _animator;
        private Enemy _owner;

        public Renderer[] Renderers { get; private set; }

        public void Initialize(Enemy enemy)
        {
            _owner = enemy;
            _animator = GetComponent<Animator>();
            Renderers = GetComponentsInChildren<Renderer>();
        }

        public void SetParam(int animHash , bool value) => _animator.SetBool(animHash, value);
        public void SetParam(int animHash , int value) => _animator.SetInteger(animHash, value);
        public void SetParam(int animHash , float value) => _animator.SetFloat(animHash, value);
        
    }
}