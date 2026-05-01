using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Information.Code;

namespace Work.Enemies.Code
{
    public class EnemyEvent { }

    public struct EnemyHitEvent : IEvent
    {
        public GameObject Target { get; }
        public InfoDataSO Info { get; }
        public EnemyHitEvent(GameObject target, InfoDataSO info)
        {
            Target = target;
            Info = info;
        }
    }
}
