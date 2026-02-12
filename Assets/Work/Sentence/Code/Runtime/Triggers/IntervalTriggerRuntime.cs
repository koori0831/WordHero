using System;
using UnityEngine;

namespace Work.Sentence.Code.Runtime.Triggers
{
    public sealed class IntervalTriggerRuntime : ISentenceTriggerRuntime
    {
        private readonly GameObject _owner;
        private readonly float _intervalSeconds;
        private float _time;
        private Action<SentenceTriggerSignal> _onTriggered;

        public IntervalTriggerRuntime(GameObject owner, float intervalSeconds)
        {
            _owner = owner;
            _intervalSeconds = intervalSeconds <= 0f ? 0.1f : intervalSeconds;
        }

        public void Bind(Action<SentenceTriggerSignal> onTriggered)
        {
            _onTriggered = onTriggered;
            _time = 0f;
        }

        public void Tick(float deltaTime)
        {
            _time += deltaTime;
            if (_time < _intervalSeconds) return;

            _time -= _intervalSeconds;
            _onTriggered?.Invoke(new SentenceTriggerSignal(_owner, _owner, 0, false));
        }

        public void Dispose()
        {
            _onTriggered = null;
        }
    }
}

