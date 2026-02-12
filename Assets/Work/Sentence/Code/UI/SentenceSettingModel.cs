using Work.Sentence.Code.Data;

namespace Work.Sentence.Code.UI
{
    public readonly struct SentenceSettingSnapshot
    {
        public readonly string PartName;
        public readonly string CoreWord;
        public readonly string WordA;
        public readonly string WordB;

        public SentenceSettingSnapshot(string partName, string coreWord, string wordA, string wordB)
        {
            PartName = partName;
            CoreWord = coreWord;
            WordA = wordA;
            WordB = wordB;
        }
    }

    public interface ISentenceSettingModel
    {
        bool IsOpen { get; }
        void ToggleOpen();
        SentenceSettingSnapshot BuildSnapshot();
    }

    public sealed class SentenceSettingModel : ISentenceSettingModel
    {
        private readonly SentencePartDefinitionSO _partDefinition;

        public bool IsOpen { get; private set; }

        public SentenceSettingModel(SentencePartDefinitionSO partDefinition)
        {
            _partDefinition = partDefinition;
        }

        public void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        public SentenceSettingSnapshot BuildSnapshot()
        {
            if (_partDefinition == null)
            {
                return new SentenceSettingSnapshot("N/A", "-", "-", "-");
            }

            ModifierWordSO[] modifiers = _partDefinition.ModifierWords;
            string wordA = modifiers != null && modifiers.Length > 0 && modifiers[0] != null
                ? modifiers[0].DisplayName
                : "-";
            string wordB = modifiers != null && modifiers.Length > 1 && modifiers[1] != null
                ? modifiers[1].DisplayName
                : "-";

            return new SentenceSettingSnapshot(
                _partDefinition.BodyPart.ToString(),
                _partDefinition.CoreWord != null ? _partDefinition.CoreWord.DisplayName : "-",
                wordA,
                wordB);
        }
    }
}

