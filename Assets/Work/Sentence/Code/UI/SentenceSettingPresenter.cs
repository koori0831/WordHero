using System;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Sentence.Code.UI
{
    public sealed class SentenceSettingPresenter : IDisposable
    {
        private readonly ISentenceSettingModel _model;
        private readonly SentenceSettingView _view;
        private readonly IEventBinding _menuBinding;

        public SentenceSettingPresenter(ISentenceSettingModel model, SentenceSettingView view)
        {
            _model = model;
            _view = view;
            _menuBinding = EventBinding.Bind<InputMenuEvent>(OnMenuEvent);

            Render();
            _view.SetVisible(_model.IsOpen);
        }

        public void Dispose()
        {
            _menuBinding?.Dispose();
        }

        private void OnMenuEvent(InputMenuEvent evt)
        {
            _model.ToggleOpen();
            Render();
        }

        private void Render()
        {
            SentenceSettingSnapshot snapshot = _model.BuildSnapshot();
            _view.SetPartName(snapshot.PartName);
            _view.SetCoreWord(snapshot.CoreWord);
            _view.SetWordA(snapshot.WordA);
            _view.SetWordB(snapshot.WordB);
            _view.SetVisible(_model.IsOpen);
        }
    }
}

