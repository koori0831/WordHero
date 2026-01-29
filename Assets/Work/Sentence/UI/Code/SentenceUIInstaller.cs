using UnityEngine;
using Work.Sentence.Code;

namespace Work.Sentence.UI.Code
{
	public class SentenceUIInstaller : MonoBehaviour
	{
        [SerializeField] private CurrentSentenceView _view;
        [SerializeField] private SentenceSystemMono _sentenceSystem;

        private CurrentSentencePresenter _presenter;

        private void Start()
        {
            var model = new CurrentSentenceModel();
            _view.Bind(model);

            _presenter = new CurrentSentencePresenter(model);
            _presenter.Bind(_sentenceSystem.Builder);
        }

        private void OnDestroy() => _presenter?.Dispose();
    }
}