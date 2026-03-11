using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Sentence.Code.Data;
using Work.Sentence.Code.Runtime.Ports;

namespace Work.Sentence.Code.Runtime
{
    public sealed class SentenceSystem : IDisposable
    {
        private readonly GameObject _owner;
        private readonly SentenceController[] _controllers;
        private readonly SentencePortPreviewService _previewService;

        public SentencePortPreviewService PreviewService => _previewService;

        public SentenceSystem(GameObject owner, IPortCompatibilityPolicy policy)
        {
            _owner = owner;
            _controllers = new SentenceController[Enum.GetValues(typeof(BodyPart)).Length];
            _previewService = new SentencePortPreviewService(policy);
        }

        public void InstallLoadout(SentenceLoadoutSO loadout, IPortCompatibilityPolicy policy)
        {
            Clear();
            if (loadout == null || loadout.Parts == null) return;

            List<PortCompatibilityResult> issues = new List<PortCompatibilityResult>(4);

            for (int i = 0; i < loadout.Parts.Length; i++)
            {
                SentencePartDefinitionSO definition = loadout.Parts[i];
                if (definition == null) continue;

                issues.Clear();
                if (!SentenceController.TryCreate(definition, _owner, policy, issues, out SentenceController controller))
                {
                    if (issues.Count > 0)
                    {
                        Debug.LogWarning($"Sentence install blocked. Part={definition.BodyPart}, Reason={issues[0].Reason}");
                    }
                    continue;
                }

                _controllers[(int)definition.BodyPart] = controller;
            }
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _controllers.Length; i++)
            {
                _controllers[i]?.Tick(deltaTime);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _controllers.Length; i++)
            {
                if (_controllers[i] == null) continue;
                _controllers[i].Dispose();
                _controllers[i] = null;
            }
        }

        public void Dispose()
        {
            Clear();
        }
    }
}

