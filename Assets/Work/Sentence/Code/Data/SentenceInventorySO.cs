using UnityEngine;
using System.Collections.Generic;
using System;

namespace Work.Sentence.Code.Data
{
    [Serializable]
    public record InventoryWord{public ModifierWordSO wordSO; public bool isUse; };

    [CreateAssetMenu(fileName = "SentenceInventory", menuName = "SO/Sentence/SentenceInventory")]
    public class SentenceInventorySO : ScriptableObject
    {
        public List<InventoryWord> Words = new List<InventoryWord>();
    }
}