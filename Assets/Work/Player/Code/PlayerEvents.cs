using UnityEngine;
using System.Collections;
using Work.Core.Utils.EventBus;

namespace Work.Player.Code
{
    public readonly record struct PlayerRequestAttackEvent : IEvent;
    public readonly record struct PlayerRequestDodgeEvent : IEvent;
}