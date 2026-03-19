using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Agents.Code;

namespace Work.Information.Code
{
    public abstract class HpBarInfoData : InfoDataSO
    {
        public HpValue HpValue { get; protected set; }
        public StatusValue StatusValue { get; protected set; }

        public HpBarInfoData GetInfo(HpValue hp, StatusValue status)
        {
            HpBarInfoData data = base.GetInfo() as HpBarInfoData;
            data.HpValue = hp;
            data.StatusValue = status;
            return data;
        }
    }
}
