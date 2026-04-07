using Work.Players.Code;
using Work.Weapons.Imprint.Code;

namespace Work.Interaction.Code
{
    public class ImprintWordCollectAction : ICollectAction
    {
        public ImprintWordSO ImprintWord;

        public void Collect(Player collector)
        {
            collector.GetImprintWord(ImprintWord, 1);
        }

        public void Initialize()
        {
        }
    }
}