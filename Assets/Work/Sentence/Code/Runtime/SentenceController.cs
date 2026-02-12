using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Sentence.Code.Data;
using Work.Sentence.Code.Runtime.Effects;
using Work.Sentence.Code.Runtime.Ports;
using Work.Sentence.Code.Runtime.Triggers;

namespace Work.Sentence.Code.Runtime
{
    public sealed class SentenceController : IDisposable
    {
        private readonly BodyPart _bodyPart;
        private readonly GameObject _owner;
        private readonly CoreWordSO _coreWord;
        private readonly ModifierWordSO[] _modifiers;
        private readonly ISentenceTriggerRuntime _triggerRuntime;

        public BodyPart BodyPart => _bodyPart;
        public CoreWordSO CoreWord => _coreWord;
        public ModifierWordSO[] Modifiers => _modifiers;

        public SentenceController(
            BodyPart bodyPart,
            GameObject owner,
            CoreWordSO coreWord,
            ModifierWordSO[] modifiers,
            ISentenceTriggerRuntime triggerRuntime)
        {
            _bodyPart = bodyPart;
            _owner = owner;
            _coreWord = coreWord;
            _modifiers = modifiers;
            _triggerRuntime = triggerRuntime;

            _triggerRuntime?.Bind(OnTriggered);
        }

        public static bool TryCreate(
            SentencePartDefinitionSO definition,
            GameObject owner,
            IPortCompatibilityPolicy policy,
            List<PortCompatibilityResult> issues,
            out SentenceController controller)
        {
            controller = null;
            if (definition == null || definition.CoreWord == null) return false;

            SentenceCompositionValidator validator = new SentenceCompositionValidator(policy);
            if (!validator.Validate(definition.CoreWord, definition.ModifierWords, issues))
            {
                return false;
            }

            ISentenceTriggerRuntime triggerRuntime = definition.CoreWord.Trigger != null
                ? definition.CoreWord.Trigger.CreateRuntime(owner)
                : null;

            controller = new SentenceController(
                definition.BodyPart,
                owner,
                definition.CoreWord,
                definition.ModifierWords,
                triggerRuntime);

            return true;
        }

        public void Tick(float deltaTime)
        {
            _triggerRuntime?.Tick(deltaTime);
        }

        public void Dispose()
        {
            _triggerRuntime?.Dispose();
        }

        private void OnTriggered(SentenceTriggerSignal signal)
        {
            SentenceEffectPipeline.Execute(_owner, _bodyPart, _modifiers, in signal);
        }
    }
}

