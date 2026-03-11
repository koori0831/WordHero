using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public enum InputDeviceType
    {
        KeyboardMouse,
        Gamepad
    }

    public readonly record struct PlayerInputEnableEvent(bool Enable) : IEvent;

    public readonly record struct InputInteractEvent : IEvent;
    public readonly record struct InputMenuEvent : IEvent;

    public readonly record struct InputDodgeEvent : IEvent;
    public readonly record struct InputAttackEvent : IEvent;

    public readonly record struct WeaponSwapEvent : IEvent;
    public readonly record struct FirstWeaponSkillEvent : IEvent;
    public readonly record struct SecondWeaponSkillEvent : IEvent;

    public readonly record struct InputDeviceChangedEvent(InputDeviceType NewDevice) : IEvent;
}