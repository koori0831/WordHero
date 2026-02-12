using UnityEngine;
using Work.Sentence.Code.Runtime;

namespace Work.Sentence.Code.Runtime.Effects
{
    public struct SentenceEffectBuildContext
    {
        public GameObject Owner;
        public BodyPart BodyPart;
        public SentenceTriggerSignal Signal;
        public int FlatPower;
        public float Multiplier;
    }

    public readonly struct SentenceEffectFireContext
    {
        public readonly GameObject Owner;
        public readonly BodyPart BodyPart;
        public readonly SentenceTriggerSignal Signal;
        public readonly int FlatPower;
        public readonly float Multiplier;

        public SentenceEffectFireContext(GameObject owner, BodyPart bodyPart, SentenceTriggerSignal signal, int flatPower, float multiplier)
        {
            Owner = owner;
            BodyPart = bodyPart;
            Signal = signal;
            FlatPower = flatPower;
            Multiplier = multiplier;
        }
    }
}

