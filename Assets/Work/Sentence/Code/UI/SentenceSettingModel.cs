using Work.Sentence.Code.Data;
using System.Text;

namespace Work.Sentence.Code.UI
{
    public readonly struct SentenceSettingSnapshot
    {
        public readonly string PartName;
        public readonly string CoreWord;
        public readonly string WordA;
        public readonly string WordB;
        public readonly string InventoryWords;

        public SentenceSettingSnapshot(string partName, string coreWord, string wordA, string wordB, string inventoryWords)
        {
            PartName = partName;
            CoreWord = coreWord;
            WordA = wordA;
            WordB = wordB;
            InventoryWords = inventoryWords;
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
        private readonly SentenceInventorySO _inventory;
        private readonly StringBuilder _builder = new StringBuilder(256);

        public bool IsOpen { get; private set; }

        public SentenceSettingModel(SentencePartDefinitionSO partDefinition, SentenceInventorySO inventory)
        {
            _partDefinition = partDefinition;
            _inventory = inventory;
        }

        public void ToggleOpen()
        {
            IsOpen = !IsOpen;
        }

        public SentenceSettingSnapshot BuildSnapshot()
        {
            if (_partDefinition == null)
            {
                return new SentenceSettingSnapshot("N/A", "-", "-", "-", BuildInventoryWords());
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
                wordB,
                BuildInventoryWords());
        }

        private string BuildInventoryWords()
        {
            if (_inventory == null || _inventory.Words == null || _inventory.Words.Count == 0)
            {
                return "No words in inventory";
            }

            _builder.Clear();
            for (int i = 0; i < _inventory.Words.Count; i++)
            {
                InventoryWord entry = _inventory.Words[i];
                if (entry == null || entry.wordSO == null) continue;

                if (_builder.Length > 0)
                {
                    _builder.Append('\n');
                }

                _builder.Append("- ");
                _builder.Append(entry.wordSO.DisplayName);
                if (entry.isUse)
                {
                    _builder.Append(" (Equipped)");
                }
            }

            if (_builder.Length == 0)
            {
                return "No words in inventory";
            }

            return _builder.ToString();
        }
    }
}

