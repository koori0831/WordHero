using UnityEngine;
using Work.Sentence.Code.Data;
using Work.Sentence.Code.Runtime;
using Work.Sentence.Code.Runtime.Ports;

namespace Work.Sentence.Code.Mono
{
    public class SentenceSystemMono : MonoBehaviour
    {
        [SerializeField] private SentenceLoadoutSO loadout;
        [SerializeField] private PortRuleSetSO portRuleSet;
        [SerializeField] private GameObject ownerOverride;

        private SentenceSystem _runtime;
        private IPortCompatibilityPolicy _portPolicy;

        public SentencePortPreviewService PreviewService => _runtime?.PreviewService;

        private void Awake()
        {
            GameObject owner = ownerOverride != null ? ownerOverride : gameObject;
            _portPolicy = BuildPortPolicy();
            _runtime = new SentenceSystem(owner, _portPolicy);
            _runtime.InstallLoadout(loadout, _portPolicy);
        }

        private void Update()
        {
            _runtime?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _runtime?.Dispose();
            _runtime = null;
        }

        private IPortCompatibilityPolicy BuildPortPolicy()
        {
            DefaultPortCompatibilityPolicy defaultPolicy = new DefaultPortCompatibilityPolicy();
            if (portRuleSet == null || portRuleSet.Rules == null || portRuleSet.Rules.Length == 0)
            {
                return defaultPolicy;
            }

            return new RuleBasedPortCompatibilityPolicy(defaultPolicy, portRuleSet.Rules);
        }
    }
}

