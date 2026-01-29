using System.Collections.Generic;

namespace Work.Sentence.Code
{
    public sealed class SentenceDraft
    {
        public WordDefinitionSO Subject;
        public WordDefinitionSO Object;
        public WordDefinitionSO Verb;
        public WordDefinitionSO Option;

        public float TotalCastTime { get; private set; }
        public readonly List<PartOfSpeech> CommitOrder = new();

        public bool IsEmpty => Subject == null && Object == null && Verb == null && Option == null;

        public void Clear()
        {
            Subject = null; Object = null; Verb = null; Option = null;
            TotalCastTime = 0f;
            CommitOrder.Clear();
        }

        public void Commit(WordDefinitionSO w)
        {
            switch (w.PartOfSpeech)
            {
                case PartOfSpeech.Subject: Subject = w; break;
                case PartOfSpeech.Object: Object = w; break;
                case PartOfSpeech.Verb: Verb = w; break;
                case PartOfSpeech.Option: Option = w; break;
            }

            TotalCastTime += w.CastTime;
            CommitOrder.Add(w.PartOfSpeech);
        }

        public bool IsComplete(bool requireSubject, bool requireObject)
        {
            if (Verb == null) return false;
            if (requireSubject && Subject == null) return false;
            if (requireObject && Object == null) return false;
            return true;
        }

        public bool IsProperInputOrderSOV()
        {
            int s = CommitOrder.IndexOf(PartOfSpeech.Subject);
            int o = CommitOrder.IndexOf(PartOfSpeech.Object);
            int v = CommitOrder.IndexOf(PartOfSpeech.Verb);
            if (s < 0 || o < 0 || v < 0) return false;
            return s < o && o < v;
        }

        public TagSet GetMergedTags()
        {
            TagSet t = default;
            if (Subject != null) t = TagSet.Merge(t, Subject.Tags);
            if (Object != null) t = TagSet.Merge(t, Object.Tags);
            if (Verb != null) t = TagSet.Merge(t, Verb.Tags);
            if (Option != null) t = TagSet.Merge(t, Option.Tags);
            return t;
        }

        public NumericToken GetDurationTokenOrNone()
        {
            if (Option == null) return NumericToken.None;
            var tok = Option.GetNumericToken();
            if (!tok.HasValue) return NumericToken.None;
            if (tok.Kind != NumericKind.DurationSeconds) return NumericToken.None;
            return tok;
        }

        public NumericToken GetChangeTokenOrNone()
        {
            var token = GetNumericTokenIfMatch(Verb, NumericKind.Percent, NumericKind.Flat);
            if (token.HasValue) return token;
            token = GetNumericTokenIfMatch(Option, NumericKind.Percent, NumericKind.Flat);
            return token;
        }

        private static NumericToken GetNumericTokenIfMatch(WordDefinitionSO word, params NumericKind[] kinds)
        {
            if (word == null) return NumericToken.None;
            var tok = word.GetNumericToken();
            if (!tok.HasValue) return NumericToken.None;
            for (int i = 0; i < kinds.Length; i++)
            {
                if (tok.Kind == kinds[i]) return tok;
            }
            return NumericToken.None;
        }
    }

    public sealed class ResolvedSentence
    {
        public SentenceDraft Draft;
        public SentenceTemplateSO Template;
        public int Score;
        public bool ProperBonus;

        public ResolvedSentence(SentenceDraft draft, SentenceTemplateSO template, int score, bool properBonus)
        {
            Draft = draft;
            Template = template;
            Score = score;
            ProperBonus = properBonus;
        }
    }
}
