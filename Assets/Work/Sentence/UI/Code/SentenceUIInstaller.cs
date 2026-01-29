using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Sentence.Code;

namespace Work.Sentence.UI.Code
{
	public class SentenceUIInstaller : MonoBehaviour
	{
        [SerializeField] private CurrentSentenceView _currentSentenceView;
        [SerializeField] private WordPaletteView _wordPaletteView;
        [SerializeField] private SentenceSystemMono _sentenceSystem;

        private CurrentSentencePresenter _currentSentencePresenter;
        private WordPalettePresenter _wordPalettePresenter;

        private void Start()
        {
            var model = new CurrentSentenceModel();
            _currentSentenceView.Bind(model);

            _currentSentencePresenter = new CurrentSentencePresenter(model);
            _currentSentencePresenter.Bind(_sentenceSystem.Builder);

            _wordPalettePresenter = new WordPalettePresenter(_wordPaletteView);
            Bus<ChangeWordPaletteEvent>.Events += HandleChangeWordPalette;
        }

        private void HandleChangeWordPalette(ChangeWordPaletteEvent evt)
        {
            _wordPalettePresenter.ChangePalette(evt.index, evt.wordPalette);
        }

        private void OnDestroy()
        {
            _currentSentencePresenter?.Dispose();
            Bus<ChangeWordPaletteEvent>.Events -= HandleChangeWordPalette;
        }
    }
}