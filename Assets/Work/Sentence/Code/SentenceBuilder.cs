using System;
using UnityEngine;

namespace Work.Sentence.Code
{
    public sealed class SentenceBuilder
    {
        public event Action<SentenceDraft> OnDraftChanged;
        public event Action<SentenceDraft> OnSentenceCompleted;
        public event Action OnCanceled;
        public event Action<DirectActionType> OnDirectAction;

        private readonly SentenceDraft _draft = new();
        private readonly float _idleCancelSeconds;
        private readonly bool _requireSubject;
        private readonly bool _requireObject;

        private float _lastChangeTime;

        private int _castingSlotIndex = -1;
        private WordDefinitionSO _castingWord;
        private float _castingElapsed;

        public SentenceDraft Current => _draft;

        public SentenceBuilder(float idleCancelSeconds = 10f, bool requireSubject = false, bool requireObject = true)
        {
            _idleCancelSeconds = idleCancelSeconds;
            _requireSubject = requireSubject;
            _requireObject = requireObject;
            _lastChangeTime = Time.time;
        }

        public void Tick(float dt)
        {
            // Idle auto-cancel
            if (!_draft.IsEmpty && Time.time - _lastChangeTime >= _idleCancelSeconds)
            {
                Cancel();
                return;
            }

            // Casting progress
            if (_castingWord != null)
            {
                _castingElapsed += dt;
                if (_castingElapsed >= _castingWord.CastTime)
                {
                    CommitOrDirect(_castingWord);
                    StopCasting();
                }
            }
        }

        public void OnWordSlotPressed(int slotIndex, WordDefinitionSO word)
        {
            if (word == null) return;

            // 이미 캐스팅 중이면 MVP는 무시(나중에 큐잉/교체 정책 가능)
            if (_castingWord != null) return;

            _castingSlotIndex = slotIndex;
            _castingWord = word;
            _castingElapsed = 0f;
        }

        public void OnWordSlotReleased(int slotIndex)
        {
            if (_castingSlotIndex != slotIndex) return;
            if (_castingWord == null) return;

            // 미완료면 취소
            if (_castingElapsed < _castingWord.CastTime)
                StopCasting();
        }

        public void Cancel()
        {
            StopCasting();
            _draft.Clear();
            _lastChangeTime = Time.time;
            OnCanceled?.Invoke();
            OnDraftChanged?.Invoke(_draft);
        }

        private void CommitOrDirect(WordDefinitionSO word)
        {
            // 완성 문장 단어(기본기) 처리
            if (word.DirectAction != DirectActionType.None)
            {
                OnDirectAction?.Invoke(word.DirectAction);
                // 조합 중이었다면 리셋(정책: 즉시 취소)
                _draft.Clear();
                OnDraftChanged?.Invoke(_draft);
                _lastChangeTime = Time.time;
                return;
            }

            _draft.Commit(word);
            _lastChangeTime = Time.time;
            OnDraftChanged?.Invoke(_draft);

            if (_draft.IsComplete(_requireSubject, _requireObject))
            {
                OnSentenceCompleted?.Invoke(_draft);
                _draft.Clear();
                OnDraftChanged?.Invoke(_draft);
            }
        }

        private void StopCasting()
        {
            _castingSlotIndex = -1;
            _castingWord = null;
            _castingElapsed = 0f;
        }
    }
}