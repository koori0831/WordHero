using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public class InputContainer : Console.IPlayerActions
    {
        private Console _console;

        public Vector2 MoveVector { get; private set; }
        public bool IsMovePressed { get; private set; }
        public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.KeyboardMouse;

        // 스틱 드리프트 방지
        private const float Deadzone = 0.2f;

        public void Init()
        {
            if (_console == null)
            {
                _console = new Console();
                _console.Player.SetCallbacks(this);
            }
            _console.Player.Enable();
            Bus<InputEnableEvent>.Events += SetEnable;
        }

        public void Deinit()
        {
            _console.Player.Disable();
            _console = null;
            Bus<InputEnableEvent>.Events -= SetEnable;
        }

        public void SetEnable(InputEnableEvent evt)
        {
            if (evt.Enable) _console.Player.Enable();
            else _console.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.canceled)
            {
                MoveVector = Vector2.zero;
                IsMovePressed = false;
                return;
            }

            var v = context.ReadValue<Vector2>();

            // deadzone
            if (v.sqrMagnitude < Deadzone * Deadzone)
                v = Vector2.zero;

            MoveVector = Vector2.ClampMagnitude(v, 1f);
            IsMovePressed = MoveVector != Vector2.zero;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<InputInteractEvent>.Raise(new InputInteractEvent());
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<InputMenuEvent>.Raise(new InputMenuEvent());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<InputAttackEvent>.Raise(new InputAttackEvent());
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<InputDodgeEvent>.Raise(new InputDodgeEvent());
        }

        private void UpdateCurrentDevice(InputAction.CallbackContext context)
        {
            var device = context.control?.device;
            if (device == null)
                return;

            var newDevice = device is Gamepad ? InputDeviceType.Gamepad : InputDeviceType.KeyboardMouse;
            if (newDevice == CurrentDeviceType)
                return;

            CurrentDeviceType = newDevice;
            Bus<InputDeviceChangedEvent>.Raise(new InputDeviceChangedEvent(CurrentDeviceType));
        }
    }
}
