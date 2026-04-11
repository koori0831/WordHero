using System;
using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Imprint.Code;
using Random = UnityEngine.Random;

namespace Work.Interaction.Code
{
    [Serializable]
    public class ImprintWordRandomCollectAction : ICollectAction
    {
        public ImprintWordListSO imprintWordList;
        private ImprintWordSO imprintWord;

        public void Collect(Player collector)
        {
            collector.GetImprintWord(imprintWord, 1);
        }

        public void Initialize()
        {
            imprintWord = imprintWordList.Words[Random.Range(0, imprintWordList.Words.Count)];
        }
    }
}
