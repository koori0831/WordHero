using System;

namespace Work.Dialogue.Code
{
    [Serializable]
    public struct DialogueChoice
    {
        public string ChoiceText;
        public string NextNodeID;
    }
}
