using System.Collections.Generic;
using UnityEngine;

namespace Work.Dialogue.Code
{
    [CreateAssetMenu(fileName = "DialogueInformation", menuName = "SO/Dialogue/Information", order = 20)]
    public class DialogueInformationSO : ScriptableObject
    {
        [SerializeField] private string startNodeID;
        [SerializeField] private List<DialogueNode> dialogueNodes = new List<DialogueNode>();

        public string StartNodeID => startNodeID;
        public List<DialogueNode> DialogueNodes => dialogueNodes;

        public void SetNodes(List<DialogueNode> nodes)
        {
            dialogueNodes = nodes ?? new List<DialogueNode>();
        }

        public void SetStartNode(string nodeID)
        {
            startNodeID = nodeID;
        }
    }
}
