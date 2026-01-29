using R3;
using TMPro;
using UnityEngine;

namespace Work.Sentence.UI.Code
{
    public sealed class CurrentSentenceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _sentenceText;

        private readonly CompositeDisposable _cd = new();

        public void Bind(CurrentSentenceModel vm)
        {
            vm.DisplayText.Subscribe(text => _sentenceText.text = text).AddTo(_cd);
        }

        private void OnDestroy() => _cd.Dispose();
    }
}