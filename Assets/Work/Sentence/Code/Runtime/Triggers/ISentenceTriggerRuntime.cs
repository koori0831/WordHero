using System;

namespace Work.Sentence.Code.Runtime.Triggers
{
    public interface ISentenceTriggerRuntime : IDisposable
    {
        void Bind(Action<SentenceTriggerSignal> onTriggered);
        void Tick(float deltaTime);
    }
}

