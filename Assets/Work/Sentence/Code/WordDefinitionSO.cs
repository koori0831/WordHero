using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;
using Work.StatSystem.Code;

namespace Work.Sentence.Code
{
    [CreateAssetMenu(menuName = "SO/Words/Word Definition")]
    public class WordDefinitionSO : ScriptableObject
    {
        [Header("Grammar")]
        public PartOfSpeech PartOfSpeech;

        [Header("Identity")]
        // 가이드 메시지
        [HelpBox("품사에 맞는 접두어를 사용하세요 (subject_, object_, verb_, option_)", HelpBoxMessageType.Info)]
        [ValidateInput(nameof(ValidateId), "ID 형식이 올바르지 않습니다! 현재 품사에 맞는 접두어로 시작해야 합니다.")]
        public string Id;

        public string DisplayName;

        [Header("Casting")]
        [Range(0f, 3f)]
        public float CastTime = 0.8f;

        [Header("Tags")]
        public TagSet Tags;

        [Header("Direct Action (for '완성 문장 단어' like BasicAttack/Dodge)")]
        public DirectActionType DirectAction = DirectActionType.None;

        [Header("Numeric Token (optional, e.g. n초간, +20%)")]
        [ShowIf(nameof(showNumericTokenSettings))] // Option/Verb일 때 노출
        public bool HasNumericValue;

        [ShowIf(nameof(showNumericTokenSettings))]
        public NumericKind NumericKind = NumericKind.None;

        [ShowIf(nameof(showNumericTokenSettings))]
        public float NumericValue;

        [Header("Linked Stat (optional, e.g. AttackPower)")]
        public StatSO LinkedStat;

        private bool showNumericTokenSettings => PartOfSpeech == PartOfSpeech.Option
                                                 || PartOfSpeech == PartOfSpeech.Verb;

        // --- 유효성 검사 메서드 ---
        // true를 반환하면 정상(에러 없음), false를 반환하면 에러 메시지 출력
        private bool ValidateId(string value)
        {
            if (string.IsNullOrEmpty(value)) return true; // 비어있을 때는 에러를 띄우지 않음 (선택 사항)
            return value.StartsWith(GetRequiredPrefix());
        }

        private string GetRequiredPrefix()
        {
            return PartOfSpeech switch
            {
                PartOfSpeech.Subject => "subject_",
                PartOfSpeech.Object => "object_",
                PartOfSpeech.Verb => "verb_",
                PartOfSpeech.Option => "option_",
                _ => ""
            };
        }

        public NumericToken GetNumericToken()
        {
            if (!HasNumericValue) return NumericToken.None;
            return new NumericToken { HasValue = true, Kind = NumericKind, Value = NumericValue };
        }
    }
}
