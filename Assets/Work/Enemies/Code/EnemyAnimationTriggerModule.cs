using System;
using UnityEngine;
using Work.Agents.Code;
using Work.Enemies.Code.AttackModules;

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

        public void HandleSpearAttackTrigger()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.Attack();
        }

        public void PlaySpearPrepareEffect()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.PlayPrepareEffect();
        }

        public void ShowSpearWarningDecal()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.ShowWarningDecal();
        }

        public void HideSpearWarningDecal()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.HideWarningDecal();
        }

        public void ResetSpearCombo()
        {
            _owner.GetModule<SpearComboAttackModule>(true)?.ResetCombo();
        }
    }
}
