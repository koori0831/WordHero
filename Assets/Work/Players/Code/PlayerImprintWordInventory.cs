using System.Collections.Generic;
using Work.Weapons.Imprint.Code;

namespace Work.Players.Code
{
    public class PlayerImprintWordInventory
    {
        private Dictionary<ImprintWordSO, int> _imprintDic = new Dictionary<ImprintWordSO, int>();

        public void AddImprintWord(ImprintWordSO imprintWord, int amount)
        {
            if (_imprintDic.ContainsKey(imprintWord))
            {
                _imprintDic[imprintWord] += amount;
            }
            else
            {
                _imprintDic.Add(imprintWord, amount);
            }
        }

        public bool TryUseImprintWord(ImprintWordSO imprintWord)
        {
            _imprintDic.TryGetValue(imprintWord, out int amount);
            if (amount > 0)
            {
                _imprintDic[imprintWord] = amount - 1;
                if (_imprintDic[imprintWord] == 0)
                    _imprintDic.Remove(imprintWord);

                return true;
            }
            return false;
        }

        public List<ImprintWordSO> GetAllImprintWords()
        {
            List<ImprintWordSO> imprintWords = new List<ImprintWordSO>();
            foreach (var kvp in _imprintDic)
            {
                if (kvp.Value > 0)
                    imprintWords.Add(kvp.Key);
            }
            return imprintWords;
        }

        public int GetAmount(ImprintWordSO imprintWord)
        {
            if (imprintWord != null && _imprintDic.TryGetValue(imprintWord, out int amount))
            {
                return amount;
            }
            return 0;
        }
    }
}