using System;
using UnityEngine;
using Work.Players.Code;
using Work.Shops.Code;
using Work.Weapons.Imprint.Code;
using Random = UnityEngine.Random;

namespace Work.Shops.Code.Actions
{
    [Serializable]
    public class GetImprintWordMachineAction : IMachineAction
    {
        [SerializeField] private ImprintWordListSO imprintWordListSO;

        public void Apply(Player player)
        {
            player.GetImprintWord(imprintWordListSO.Words[Random.Range(0, imprintWordListSO.Words.Count)], 1);
        }

        public void Initialize()
        {

        }
    }
}