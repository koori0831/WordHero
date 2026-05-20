using System;
using System.Collections.Generic;
using UnityEngine;

namespace Work.Dialogue.Code
{
    [Serializable]
    public struct DialogueNode
    {
        public string NodeID;
        public string CharacterName;
        public Sprite CharacterSprite;
        public Sprite BackgroundSprite;
        public NameTagPositionType NameTagPosition;
        [TextArea] public string DialogueDetail;
        public string NextNodeID;
        public List<DialogueChoice> Choices;
    }
}
