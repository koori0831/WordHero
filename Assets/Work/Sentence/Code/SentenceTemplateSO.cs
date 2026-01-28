using UnityEngine;

namespace Work.Sentence.Code
{
    [CreateAssetMenu(menuName = "SO/Sentences/Sentence Template")]
    public class SentenceTemplateSO : ScriptableObject
    {
        [Header("Slot Requirements")]
        public bool RequireSubject = false;
        public bool RequireObject = true;
        public bool AllowOption = true;

        [Header("Tag Constraints per Slot")]
        public TagQuery SubjectTags;
        public TagQuery ObjectTags;
        public TagQuery VerbTags;
        public TagQuery OptionTags;

        [Header("Result")]
        public EffectGraphSO Effect;

        [Header("Selection")]
        public int Priority = 0;
        public int BaseScore = 0;

        public bool Matches(SentenceDraft d)
        {
            if (d == null) return false;
            if (d.Verb == null) return false;
            if (RequireSubject && d.Subject == null) return false;
            if (RequireObject && d.Object == null) return false;
            if (!AllowOption && d.Option != null) return false;

            if (d.Subject != null && !SubjectTags.Matches(d.Subject.Tags)) return false;
            if (d.Object != null && !ObjectTags.Matches(d.Object.Tags)) return false;
            if (!VerbTags.Matches(d.Verb.Tags)) return false;
            if (d.Option != null && !OptionTags.Matches(d.Option.Tags)) return false;

            return true;
        }

        public int EvaluateScore(SentenceDraft d)
        {
            int score = BaseScore + Priority * 1000;
            if (d.Subject != null) score += SubjectTags.Score(d.Subject.Tags);
            if (d.Object != null) score += ObjectTags.Score(d.Object.Tags);
            if (d.Verb != null) score += VerbTags.Score(d.Verb.Tags);
            if (d.Option != null) score += OptionTags.Score(d.Option.Tags);
            return score;
        }
    }
}