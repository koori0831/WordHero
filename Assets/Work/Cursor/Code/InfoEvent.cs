using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Information.Code;

namespace Work.Cursor.Code
{
    public class InfoEvent { }

    public struct InfoDataEvent : IEvent
    {
        public GameObject Target { get; }
        public InfoDataSO Info { get; }
        public InfoDataEvent(GameObject target, InfoDataSO info)
        {
            Target = target;
            Info = info;
        }
    }

    public struct HideInfoDataEvent : IEvent
    {
    }
}