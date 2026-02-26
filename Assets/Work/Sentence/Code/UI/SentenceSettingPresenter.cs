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
            _view.BindPartSelection(OnPreviousPartClicked, OnNextPartClicked);

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
            _view.SetInventoryItems(snapshot.InventoryItems, OnInventoryItemButtonClicked);
            _view.SetPartNavigationEnabled(_model.CanSelectPart);
            _view.SetVisible(_model.IsOpen);
        }

        private void OnInventoryItemButtonClicked(int index)
        {
            _model.ToggleInventoryItem(index);
            Render();
        }

        private void OnPreviousPartClicked()
        {
            _model.SelectPreviousPart();
            Render();
        }

        private void OnNextPartClicked()
        {
            _model.SelectNextPart();
            Render();
        }
    }
}

