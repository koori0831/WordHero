using UnityEngine;
using Work.Agents.Code;
using Work.Enemies.Code;

namespace Work.Information.Code
{
    [CreateAssetMenu(fileName = "EnemyInfoDataSO", menuName = "Scriptable Objects/EnemyInfoDataSO")]
    public class EnemyInfoDataSO : InfoDataSO
    {
        public Enemy Owner { get; private set; }
        public HpValue EnemyHpValue { get; private set; }
        public StatusValue StatusValue { get; private set; }

        public EnemyInfoDataSO GetInfo(Enemy owner)
        {
            EnemyInfoDataSO data = base.GetInfo() as EnemyInfoDataSO;
            data.Owner = owner;
            data.EnemyHpValue = owner.GetModule<EnemyHealthModule>().HpValue;
            data.StatusValue = owner.GetModule<AgentStatusModule>().StatusValue;
            return data;
        }
    }
}