namespace Work.Dialogue.Code
{
    public readonly struct DialogueChoiceViewData
    {
        public readonly int ChoiceIndex;
        public readonly string ChoiceText;
        public readonly string NextNodeID;

        public DialogueChoiceViewData(int choiceIndex, string choiceText, string nextNodeID)
        {
            ChoiceIndex = choiceIndex;
            ChoiceText = choiceText;
            NextNodeID = nextNodeID;
        }
    }
}
