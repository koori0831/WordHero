using System;
using UnityEngine;
using Work.Combat.Code;
using Work.Core.Utils.EventBus;

namespace Work.Sentence.Code.Runtime.Triggers
{
    public sealed class OnHitTriggerRuntime : ISentenceTriggerRuntime
    {
        private readonly GameObject _owner;
        private IEventBinding _binding;
        private Action<SentenceTriggerSignal> _onTriggered;

        public OnHitTriggerRuntime(GameObject owner)
        {
            _owner = owner;
        }

        public void Bind(Action<SentenceTriggerSignal> onTriggered)
        {
            _onTriggered = onTriggered;
            _binding = EventBinding.Bind<CombatHitEvent>(OnCombatHit);
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

        private void OnCombatHit(CombatHitEvent evt)
        {
            if (evt.Target != _owner) return;
            _onTriggered?.Invoke(new SentenceTriggerSignal(evt.Source, evt.Target, evt.Damage, evt.IsCritical));
        }
    }
}

