using Code.Entities;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Sentence.Code;
using Work.StatSystem.Code;

namespace Work.Player.Code
{
    public sealed class PlayerCombatExecutor : MonoBehaviour, ICombatExecutor, IEntityComponent
    {
        private Player _player;

        public Entity Owner => _player;

        public void InitCompo(Entity entity)
        {
            _player = entity as Player;
        }

        public void ExecuteDirectAction(DirectActionType action)
        {
            switch (action)
            {
                case DirectActionType.BasicAttack:
                    Bus<PlayerRequestAttackEvent>.Raise(new PlayerRequestAttackEvent());
                    break;

                case DirectActionType.Dodge:
                    Bus<PlayerRequestDodgeEvent>.Raise(new PlayerRequestDodgeEvent());
                    break;
            }
        }

        public void ExecuteSkill(SkillInstance skill)
        {
            Debug.Log($"PLAYER: Skill={skill.DebugName} kind={skill.Kind} dmg={skill.Damage} dur={skill.Duration}");

            if (skill.HasStatEffect && skill.StatEffect.IsValid)
            {
                Bus<StatApplyModifierEvent>.Raise(
                    new StatApplyModifierEvent(_player, skill.StatEffect.Stat, skill.StatEffect.Key, skill.StatEffect.Modifier));
            }

            switch (skill.Kind)
            {
                case SkillKind.Attack:
                    // TODO: Attack 실행(투사체/근접/AoE는 skill.Tags.Form로 분기 가능)
                    break;

                case SkillKind.Buff:
                    // TODO: 버프 적용(자기 자신)
                    break;

                case SkillKind.CC:
                    // TODO: 적에게 CC 적용
                    break;

                case SkillKind.Special:
                    // TODO: 메테오 같은 특수기
                    break;
            }
        }
    }
}
