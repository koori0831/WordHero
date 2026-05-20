using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Dialogue.Code.UI
{
    public class DialogueTextPresenter : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private GameObject nameTag;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("Settings")]
        [SerializeField] private float typingSpeed = 0.03f;
        [SerializeField] private float startDelay = 0.1f;

        private CancellationTokenSource _typingCts;
        private bool _isTyping;

        private void OnEnable()
        {
            Bus<DialogueProgressEvent>.Events += OnDialogueProgress;
            Bus<DialogueEndEvent>.Events += OnDialogueEnd;
            Bus<UIContinueButtonPressedEvent>.Events += OnUiContinueButtonPressed;
        }

        private void OnDisable()
        {
            Bus<DialogueProgressEvent>.Events -= OnDialogueProgress;
            Bus<DialogueEndEvent>.Events -= OnDialogueEnd;
            Bus<UIContinueButtonPressedEvent>.Events -= OnUiContinueButtonPressed;
            CancelTyping();
        }

        private void OnDialogueProgress(DialogueProgressEvent evt)
        {
            SetName(evt.CharacterName);

            if (dialogueText == null)
            {
                return;
            }

            CancelTyping();
            dialogueText.text = evt.DialogueDetail ?? string.Empty;
            dialogueText.maxVisibleCharacters = 0;

            _typingCts = new CancellationTokenSource();
            TypeDialogueAsync(_typingCts).Forget();
        }

        private void SetName(string characterName)
        {
            bool hasName = !string.IsNullOrWhiteSpace(characterName);
            if (nameTag != null)
            {
                nameTag.SetActive(hasName);
            }

            if (nameText != null)
            {
                nameText.text = hasName ? characterName : string.Empty;
            }
        }

        private void OnUiContinueButtonPressed(UIContinueButtonPressedEvent evt)
        {
            if (dialogueText == null)
            {
                Bus<ContinueDialogueEvent>.Raise(new ContinueDialogueEvent());
                return;
            }

            if (_isTyping)
            {
                CancelTyping();
                dialogueText.ForceMeshUpdate();
                dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
                Bus<DialogueTypingFinishedEvent>.Raise(new DialogueTypingFinishedEvent());
                return;
            }

            Bus<ContinueDialogueEvent>.Raise(new ContinueDialogueEvent());
        }

        private void OnDialogueEnd(DialogueEndEvent evt)
        {
            CancelTyping();
            SetName(null);

            if (dialogueText != null)
            {
                dialogueText.text = string.Empty;
                dialogueText.maxVisibleCharacters = 0;
            }

            Bus<DialogueTypingFinishedEvent>.Raise(new DialogueTypingFinishedEvent());
        }

        private async UniTaskVoid TypeDialogueAsync(CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;
            _isTyping = true;

            try
            {
                if (startDelay > 0f)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: token);
                }

                dialogueText.ForceMeshUpdate();
                int totalCharacters = dialogueText.textInfo.characterCount;
                for (int i = 0; i <= totalCharacters; i++)
                {
                    token.ThrowIfCancellationRequested();
                    dialogueText.maxVisibleCharacters = i;

                    if (i < totalCharacters && typingSpeed > 0f)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(typingSpeed), cancellationToken: token);
                    }
                }

                Bus<DialogueTypingFinishedEvent>.Raise(new DialogueTypingFinishedEvent());
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_typingCts == cts)
                {
                    _isTyping = false;
                    _typingCts.Dispose();
                    _typingCts = null;
                }
            }
        }

        private void CancelTyping()
        {
            if (_typingCts == null)
            {
                _isTyping = false;
                return;
            }

            _typingCts.Cancel();
            _typingCts.Dispose();
            _typingCts = null;
            _isTyping = false;
        }
    }
}
