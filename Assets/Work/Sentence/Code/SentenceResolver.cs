using System.Collections.Generic;

namespace Work.Sentence.Code
{
    public sealed class SentenceResolver
    {
        private readonly List<SentenceTemplateSO> _templates;
        private readonly int _properBonusScore;

        public SentenceResolver(List<SentenceTemplateSO> templates, int properBonusScore = 50)
        {
            _templates = templates;
            _properBonusScore = properBonusScore;
        }

        public ResolvedSentence Resolve(SentenceDraft draft)
        {
            SentenceTemplateSO best = null;
            int bestScore = int.MinValue;

            foreach (var t in _templates)
            {
                if (t == null) continue;
                if (!t.Matches(draft)) continue;

                int score = t.EvaluateScore(draft);

                bool proper = draft.IsProperInputOrderSOV();
                if (proper) score += _properBonusScore;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = t;
                }
            }

            if (best == null) return null;
            return new ResolvedSentence(draft, best, bestScore, draft.IsProperInputOrderSOV());
        }
    }
}