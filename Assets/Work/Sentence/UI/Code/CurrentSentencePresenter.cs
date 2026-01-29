using System;
using System.Linq;
using Work.Sentence.Code;

namespace Work.Sentence.UI.Code
{
    public class CurrentSentencePresenter : IDisposable
    {
        private readonly CurrentSentenceModel _model;
        private SentenceBuilder _builder;

        public CurrentSentencePresenter(CurrentSentenceModel model)
        {
            _model = model;
        }

        public void Bind(SentenceBuilder builder)
        {
            _builder = builder;
            _builder.OnDraftChanged += OnDraftChanged;
            _builder.OnCanceled += OnCanceled;
        }

        private void OnDraftChanged(SentenceDraft draft)
        {
            var option = draft.Option?.DisplayName ?? "";
            var subject = draft.Subject?.DisplayName ?? "";
            var obj = draft.Object?.DisplayName ?? "";
            var verb = draft.Verb?.DisplayName ?? "";

            _model.DisplayText.Value = Build(option, subject, obj, verb);
        }

        private void OnCanceled()
        {
            _model.DisplayText.Value = "";
        }

        private static string Build(string option, string subject, string obj, string verb)
        {
            // 표시 순서: Option + Subject + Object + Verb  (SOV + 옵션 1개)
            // 빈 값은 자동으로 스킵
            return string.Join(" ",
                new[] { option, subject, obj, verb }.Where(s => !string.IsNullOrWhiteSpace(s))
            );
        }

        public void Dispose()
        {
            if (_builder != null)
            {
                _builder.OnDraftChanged -= OnDraftChanged;
                _builder.OnCanceled -= OnCanceled;
                _builder = null;
            }
        }
    }
}