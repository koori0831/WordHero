using Work.Players.Code;

namespace Work.Interaction.Code
{
	public interface ICollectAction
	{
		public void Initialize();
        public void Collect(Player collector);
    }
}