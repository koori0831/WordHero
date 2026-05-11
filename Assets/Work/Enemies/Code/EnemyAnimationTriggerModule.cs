using System;
using UnityEngine;
using Work.Agents.Code;

namespace Work.Enemies.Code
{
    public class EnemyAnimationTriggerModule : MonoBehaviour, IAgentModule
    {
        private Enemy _owner;

        public Action OnAnimationEnd;
        public Action OnAttackEvent;

        public void Initialize(Agent agent)
        {
            _owner = agent as Enemy;
        }

        public void AnimationEndTrigger()
        {
            OnAnimationEnd?.Invoke();
        }

        public void HandleAttackTrigger()
        {
            OnAttackEvent?.Invoke();
        }
    }
}
