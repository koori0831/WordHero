using System;
using Work.Players.Code;

namespace Work.Shops.Code
{
    public interface IMachineAction 
    {
        public void Initialize();
        public void Apply(Player player);
    }
}
