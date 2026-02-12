using System;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Player.Code.States;

namespace Work.Sentence.Code.Runtime.Triggers
{
    public sealed class OnDodgeTriggerRuntime : ISentenceTriggerRuntime
    {
        private readonly GameObject _owner;
        private IEventBinding _binding;
        private Action<SentenceTriggerSignal> _onTriggered;

        public OnDodgeTriggerRuntime(GameObject owner)
        {
            _owner = owner;
        }

        public void Bind(Action<SentenceTriggerSignal> onTriggered)
        {
            _onTriggered = onTriggered;
            _binding = EventBinding.Bind<DodgeEvent>(OnDodge);
        }

        public void Tick(float deltaTime)
        {
        }

        public void Dispose()
        {
            _binding?.Dispose();
            _binding = null;
            _onTriggered = null;
        }

        private void OnDodge(DodgeEvent evt)
        {
            _onTriggered?.Invoke(new SentenceTriggerSignal(_owner, _owner, 0, false));
        }
    }
}

