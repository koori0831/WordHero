using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.AcquireItem.Code
{
    internal class AcquireItemEvents
    {
    }

    public record struct OnGetItemEvent(string Name, string Type, Color Color) : IEvent;
}
