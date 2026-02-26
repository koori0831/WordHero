using UnityEngine;
using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.UI
{
    public class SentenceSettingUIBootstrapper : MonoBehaviour
    {
        [SerializeField] private SentenceSettingView view;
        [SerializeField] private SentenceLoadoutSO loadout;
        [SerializeField] private SentencePartDefinitionSO targetPart;
        [SerializeField] private SentenceInventorySO inventory;

        private SentenceSettingPresenter _presenter;

        private void Awake()
        {
            if (view == null)
            {
                Debug.LogError("SentenceSettingView is not assigned.", this);
                enabled = false;
                return;
            }

            ISentenceSettingModel model = new SentenceSettingModel(loadout, targetPart, inventory);
            _presenter = new SentenceSettingPresenter(model, view);
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            _presenter = null;
        }
    }
}

