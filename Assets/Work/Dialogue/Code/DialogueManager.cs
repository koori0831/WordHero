using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code
{
    public class DialogueManager : MonoBehaviour
    {
        private enum DialogueState
        {
            Idle,
            Active
        }

        public static DialogueManager Instance { get; private set; }

        private readonly Dictionary<string, DialogueNode> _nodeCache = new Dictionary<string, DialogueNode>();

        private DialogueState _state = DialogueState.Idle;
        private DialogueInformationSO _dialogueInformation;
        private string _currentNodeID;
        private bool _isAcceptingInput;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Bus<DialogueStartEvent>.Events += OnDialogueStart;
            Bus<ContinueDialogueEvent>.Events += OnContinueDialogue;
            Bus<DialogueSkipEvent>.Events += OnDialogueSkip;
            Bus<DialogueChoiceSelectedEvent>.Events += OnChoiceSelected;
        }

        private void OnDisable()
        {
            Bus<DialogueStartEvent>.Events -= OnDialogueStart;
            Bus<ContinueDialogueEvent>.Events -= OnContinueDialogue;
            Bus<DialogueSkipEvent>.Events -= OnDialogueSkip;
            Bus<DialogueChoiceSelectedEvent>.Events -= OnChoiceSelected;
        }

        private void OnDialogueStart(DialogueStartEvent evt)
        {
            if (evt.DialogueSO == null || evt.DialogueSO.DialogueNodes == null || evt.DialogueSO.DialogueNodes.Count == 0)
            {
                Debug.LogError("Cannot start dialogue: DialogueInformationSO is null or empty.");
                return;
            }

            _dialogueInformation = evt.DialogueSO;
            _nodeCache.Clear();

            foreach (DialogueNode node in _dialogueInformation.DialogueNodes)
            {
                if (string.IsNullOrWhiteSpace(node.NodeID))
                {
                    continue;
                }

                _nodeCache[node.NodeID] = node;
            }

            if (_nodeCache.Count == 0)
            {
                Debug.LogError("Cannot start dialogue: no node has a valid NodeID.");
                ResetDialogue();
                return;
            }

            _currentNodeID = ResolveStartNodeID(_dialogueInformation);
            _state = DialogueState.Active;
            ProcessCurrentNode();
        }

        private string ResolveStartNodeID(DialogueInformationSO dialogueInformation)
        {
            if (!string.IsNullOrWhiteSpace(dialogueInformation.StartNodeID) && _nodeCache.ContainsKey(dialogueInformation.StartNodeID))
            {
                return dialogueInformation.StartNodeID;
            }

            for (int i = 0; i < dialogueInformation.DialogueNodes.Count; i++)
            {
                string nodeID = dialogueInformation.DialogueNodes[i].NodeID;
                if (!string.IsNullOrWhiteSpace(nodeID) && _nodeCache.ContainsKey(nodeID))
                {
                    return nodeID;
                }
            }

            return null;
        }

        private void ProcessCurrentNode()
        {
            _isAcceptingInput = false;

            if (string.IsNullOrWhiteSpace(_currentNodeID) || !_nodeCache.TryGetValue(_currentNodeID, out DialogueNode currentNode))
            {
                Debug.LogError($"Dialogue node '{_currentNodeID}' not found.");
                EndDialogue();
                return;
            }

            bool hasChoices = currentNode.Choices != null && currentNode.Choices.Count > 0;

            Bus<DialogueProgressEvent>.Raise(new DialogueProgressEvent(
                currentNode.DialogueDetail,
                currentNode.CharacterName,
                currentNode.CharacterSprite,
                currentNode.BackgroundSprite,
                currentNode.NameTagPosition,
                hasChoices));

            if (hasChoices)
            {
                List<DialogueChoiceViewData> choiceViewData = new List<DialogueChoiceViewData>(currentNode.Choices.Count);
                for (int i = 0; i < currentNode.Choices.Count; i++)
                {
                    DialogueChoice choice = currentNode.Choices[i];
                    choiceViewData.Add(new DialogueChoiceViewData(i, choice.ChoiceText, choice.NextNodeID));
                }

                Bus<DialogueShowChoiceEvent>.Raise(new DialogueShowChoiceEvent(choiceViewData));
            }

            _isAcceptingInput = true;
        }

        private void OnContinueDialogue(ContinueDialogueEvent evt)
        {
            if (_state != DialogueState.Active || !_isAcceptingInput)
            {
                return;
            }

            if (!_nodeCache.TryGetValue(_currentNodeID, out DialogueNode currentNode))
            {
                EndDialogue();
                return;
            }

            if (currentNode.Choices != null && currentNode.Choices.Count > 0)
            {
                return;
            }

            MoveToNextNodeOrEnd(currentNode.NextNodeID);
        }

        private void OnChoiceSelected(DialogueChoiceSelectedEvent evt)
        {
            if (_state != DialogueState.Active || !_nodeCache.TryGetValue(_currentNodeID, out DialogueNode currentNode))
            {
                return;
            }

            if (currentNode.Choices == null || evt.ChoiceIndex < 0 || evt.ChoiceIndex >= currentNode.Choices.Count)
            {
                Debug.LogWarning($"Invalid dialogue choice index '{evt.ChoiceIndex}' at node '{_currentNodeID}'.");
                return;
            }

            MoveToNextNodeOrEnd(currentNode.Choices[evt.ChoiceIndex].NextNodeID);
        }

        private void MoveToNextNodeOrEnd(string nextNodeID)
        {
            if (string.IsNullOrWhiteSpace(nextNodeID))
            {
                EndDialogue();
                return;
            }

            if (!_nodeCache.ContainsKey(nextNodeID))
            {
                Debug.LogError($"Dialogue node '{nextNodeID}' not found.");
                EndDialogue();
                return;
            }

            _currentNodeID = nextNodeID;
            ProcessCurrentNode();
        }

        private void OnDialogueSkip(DialogueSkipEvent evt)
        {
            if (_state != DialogueState.Active)
            {
                return;
            }

            EndDialogue();
        }

        private void EndDialogue()
        {
            DialogueInformationSO endedDialogue = _dialogueInformation;
            ResetDialogue();
            Bus<DialogueEndEvent>.Raise(new DialogueEndEvent(endedDialogue));
        }

        private void ResetDialogue()
        {
            _state = DialogueState.Idle;
            _dialogueInformation = null;
            _currentNodeID = null;
            _isAcceptingInput = false;
            _nodeCache.Clear();
        }
    }
}
