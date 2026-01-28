namespace Work.Sentence.Code
{
    public interface ICombatExecutor
    {
        void ExecuteDirectAction(DirectActionType action);
        void ExecuteSkill(SkillInstance skill);
    }
}