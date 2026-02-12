using Work.Sentence.Code.Data;
using Work.Sentence.Code.Runtime;
using UnityEngine;

namespace Work.Sentence.Code.Runtime.Effects
{
    public static class SentenceEffectPipeline
    {
        public static void Execute(GameObject owner, BodyPart bodyPart, ModifierWordSO[] modifiers, in SentenceTriggerSignal signal)
        {
            SentenceEffectBuildContext buildContext = new SentenceEffectBuildContext
            {
                Owner = owner,
                BodyPart = bodyPart,
                Signal = signal,
                FlatPower = 0,
                Multiplier = 1f,
            };

            BuildEffects(modifiers, ref buildContext);
            FireEffects(modifiers, in buildContext);
        }

        private static void BuildEffects(ModifierWordSO[] modifiers, ref SentenceEffectBuildContext context)
        {
            if (modifiers == null) return;

            for (int i = 0; i < modifiers.Length; i++)
            {
                ModifierWordSO word = modifiers[i];
                if (word == null) continue;

                SentenceEffectSO[] effects = word.Effects;
                if (effects == null) continue;

                for (int j = 0; j < effects.Length; j++)
                {
                    SentenceEffectSO effect = effects[j];
                    if (effect == null) continue;
                    effect.Build(ref context);
                }
            }
        }

        private static void FireEffects(ModifierWordSO[] modifiers, in SentenceEffectBuildContext buildContext)
        {
            if (modifiers == null) return;

            SentenceEffectFireContext fireContext = new SentenceEffectFireContext(
                buildContext.Owner,
                buildContext.BodyPart,
                buildContext.Signal,
                buildContext.FlatPower,
                buildContext.Multiplier);

            for (int i = 0; i < modifiers.Length; i++)
            {
                ModifierWordSO word = modifiers[i];
                if (word == null) continue;

                SentenceEffectSO[] effects = word.Effects;
                if (effects == null) continue;

                for (int j = 0; j < effects.Length; j++)
                {
                    SentenceEffectSO effect = effects[j];
                    if (effect == null) continue;
                    effect.Fire(in fireContext);
                }
            }
        }
    }
}
