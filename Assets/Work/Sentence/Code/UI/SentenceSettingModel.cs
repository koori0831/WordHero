using Work.Sentence.Code.Data;
using System.Collections.Generic;
using System;
using Work.Core.Utils.EventBus;
using Work.Input.Code;

namespace Work.Sentence.Code.UI
{
    public readonly struct SentenceInventoryItemSnapshot
    {
        public readonly string WordName;
        public readonly bool IsEquipped;

        public SentenceInventoryItemSnapshot(string wordName, bool isEquipped)
        {
            WordName = wordName;
            IsEquipped = isEquipped;
        }
    }

    public readonly struct SentenceSettingSnapshot
    {
        public readonly string PartName;
        public readonly string CoreWord;
        public readonly string WordA;
        public readonly string WordB;
        public readonly IReadOnlyList<SentenceInventoryItemSnapshot> InventoryItems;

        public SentenceSettingSnapshot(
            string partName,
            string coreWord,
            string wordA,
            string wordB,
            IReadOnlyList<SentenceInventoryItemSnapshot> inventoryItems)
        {
            PartName = partName;
            CoreWord = coreWord;
            WordA = wordA;
            WordB = wordB;
            InventoryItems = inventoryItems;
        }
    }

    public interface ISentenceSettingModel
    {
        bool IsOpen { get; }
        void ToggleOpen();
        bool CanSelectPart { get; }
        int PartCount { get; }
        int SelectedPartIndex { get; }
        void SelectPreviousPart();
        void SelectNextPart();
        void ToggleInventoryItem(int index);
        SentenceSettingSnapshot BuildSnapshot();
    }

    public sealed class SentenceSettingModel : ISentenceSettingModel
    {
        private const int ModifierSlotCount = 2;

        private readonly SentencePartDefinitionSO[] _parts;
        private readonly SentenceInventorySO _inventory;
        private readonly List<SentenceInventoryItemSnapshot> _inventoryItems = new List<SentenceInventoryItemSnapshot>(16);
        private readonly List<ModifierWordSO> _workingEquippedWords = new List<ModifierWordSO>(ModifierSlotCount);
        private int _selectedPartIndex;

        public bool IsOpen { get; private set; }
        public bool CanSelectPart => _parts.Length > 1;
        public int PartCount => _parts.Length;
        public int SelectedPartIndex => _selectedPartIndex;

        public SentenceSettingModel(SentencePartDefinitionSO partDefinition, SentenceInventorySO inventory)
            : this(null, partDefinition, inventory)
        {
        }

        public SentenceSettingModel(SentenceLoadoutSO loadout, SentencePartDefinitionSO fallbackPart, SentenceInventorySO inventory)
        {
            _inventory = inventory;
            _parts = BuildParts(loadout, fallbackPart);
            _selectedPartIndex = 0;
        }

        public void ToggleOpen()
        {
            IsOpen = !IsOpen;
            Bus<PlayerInputEnableEvent>.Raise(new(!IsOpen));
        }

        public void SelectPreviousPart()
        {
            if (!CanSelectPart)
            {
                return;
            }

            _selectedPartIndex = (_selectedPartIndex - 1 + _parts.Length) % _parts.Length;
        }

        public void SelectNextPart()
        {
            if (!CanSelectPart)
            {
                return;
            }

            _selectedPartIndex = (_selectedPartIndex + 1) % _parts.Length;
        }

        public void ToggleInventoryItem(int index)
        {
            if (_inventory == null || _inventory.Words == null)
            {
                return;
            }

            if (index < 0 || index >= _inventory.Words.Count)
            {
                return;
            }

            InventoryWord entry = _inventory.Words[index];
            if (entry == null || entry.wordSO == null)
            {
                return;
            }

            SentencePartDefinitionSO currentPart = GetCurrentPart();
            if (currentPart == null)
            {
                return;
            }

            BuildEquippedWords(currentPart);
            int existingIndex = _workingEquippedWords.IndexOf(entry.wordSO);
            if (existingIndex >= 0)
            {
                _workingEquippedWords.RemoveAt(existingIndex);
            }
            else
            {
                if (_workingEquippedWords.Count >= ModifierSlotCount)
                {
                    return;
                }

                _workingEquippedWords.Add(entry.wordSO);
            }

            currentPart.SetModifierWordsFromEquipped(_workingEquippedWords, ModifierSlotCount);
        }

        public SentenceSettingSnapshot BuildSnapshot()
        {
            IReadOnlyList<SentenceInventoryItemSnapshot> inventoryItems = BuildInventoryItems();
            SentencePartDefinitionSO currentPart = GetCurrentPart();

            if (currentPart == null)
            {
                return new SentenceSettingSnapshot("N/A", "-", "-", "-", inventoryItems);
            }

            ModifierWordSO[] modifiers = currentPart.ModifierWords;
            string wordA = modifiers != null && modifiers.Length > 0 && modifiers[0] != null
                ? modifiers[0].DisplayName
                : "-";
            string wordB = modifiers != null && modifiers.Length > 1 && modifiers[1] != null
                ? modifiers[1].DisplayName
                : "-";
            string partName = currentPart.BodyPart.ToString();
            if (CanSelectPart)
            {
                partName = $"{partName} ({_selectedPartIndex + 1}/{_parts.Length})";
            }

            return new SentenceSettingSnapshot(
                partName,
                currentPart.CoreWord != null ? currentPart.CoreWord.DisplayName : "-",
                wordA,
                wordB,
                inventoryItems);
        }

        private IReadOnlyList<SentenceInventoryItemSnapshot> BuildInventoryItems()
        {
            _inventoryItems.Clear();
            SentencePartDefinitionSO currentPart = GetCurrentPart();

            if (_inventory == null || _inventory.Words == null || _inventory.Words.Count == 0)
            {
                return _inventoryItems;
            }

            ModifierWordSO[] modifiers = currentPart != null ? currentPart.ModifierWords : null;
            for (int i = 0; i < _inventory.Words.Count; i++)
            {
                InventoryWord entry = _inventory.Words[i];
                if (entry == null || entry.wordSO == null) continue;

                bool isEquipped = ContainsModifier(modifiers, entry.wordSO);
                _inventoryItems.Add(new SentenceInventoryItemSnapshot(entry.wordSO.DisplayName, isEquipped));
            }

            return _inventoryItems;
        }

        private SentencePartDefinitionSO GetCurrentPart()
        {
            if (_parts == null || _parts.Length == 0)
            {
                return null;
            }

            if (_selectedPartIndex < 0 || _selectedPartIndex >= _parts.Length)
            {
                _selectedPartIndex = 0;
            }

            return _parts[_selectedPartIndex];
        }

        private void BuildEquippedWords(SentencePartDefinitionSO part)
        {
            _workingEquippedWords.Clear();
            if (part == null || part.ModifierWords == null)
            {
                return;
            }

            for (int i = 0; i < part.ModifierWords.Length; i++)
            {
                ModifierWordSO word = part.ModifierWords[i];
                if (word == null)
                {
                    continue;
                }

                _workingEquippedWords.Add(word);
                if (_workingEquippedWords.Count >= ModifierSlotCount)
                {
                    break;
                }
            }
        }

        private static bool ContainsModifier(ModifierWordSO[] modifiers, ModifierWordSO target)
        {
            if (modifiers == null || target == null)
            {
                return false;
            }

            for (int i = 0; i < modifiers.Length; i++)
            {
                if (modifiers[i] == target)
                {
                    return true;
                }
            }

            return false;
        }

        private static SentencePartDefinitionSO[] BuildParts(SentenceLoadoutSO loadout, SentencePartDefinitionSO fallbackPart)
        {
            if (loadout != null && loadout.Parts != null && loadout.Parts.Length > 0)
            {
                List<SentencePartDefinitionSO> parts = new List<SentencePartDefinitionSO>(loadout.Parts.Length);
                for (int i = 0; i < loadout.Parts.Length; i++)
                {
                    if (loadout.Parts[i] != null)
                    {
                        parts.Add(loadout.Parts[i]);
                    }
                }

                if (parts.Count > 0)
                {
                    return parts.ToArray();
                }
            }

            if (fallbackPart != null)
            {
                return new[] { fallbackPart };
            }

            return Array.Empty<SentencePartDefinitionSO>();
        }
    }
}

