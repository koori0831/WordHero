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
            if (context.performed)
                Bus<InputInteractEvent>.Raise(new InputInteractEvent());
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputMenuEvent>.Raise(new InputMenuEvent());
        }

        public void OnPaletteSwap(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputPaletteSwapEvent>.Raise(new InputPaletteSwapEvent());
        }

        public void OnSpellA(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputSpellAEvent>.Raise(new InputSpellAEvent());
        }

        public void OnSpellB(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputSpellBEvent>.Raise(new InputSpellBEvent());
        }

        public void OnWordLeft(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Bus<InputWordSellectLeftEvent>.Raise(new InputWordSellectLeftEvent(false));
            else if (context.canceled)
                Bus<InputWordSellectLeftEvent>.Raise(new InputWordSellectLeftEvent(true));
        }

        public void OnWordUp(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Bus<InputWordSellectUpEvent>.Raise(new InputWordSellectUpEvent(false));
            else if (context.canceled)
                Bus<InputWordSellectUpEvent>.Raise(new InputWordSellectUpEvent(true));
        }

        public void OnWordRight(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Bus<InputWordSellectRightEvent>.Raise(new InputWordSellectRightEvent(false));
            else if (context.canceled)
                Bus<InputWordSellectRightEvent>.Raise(new InputWordSellectRightEvent(true));
        }

        public void OnWordDown(InputAction.CallbackContext context)
        {
            if (context.started || context.performed)
                Bus<InputWordSellectDownEvent>.Raise(new InputWordSellectDownEvent(false));
            else if (context.canceled)
                Bus<InputWordSellectDownEvent>.Raise(new InputWordSellectDownEvent(true));
        }

        public void OnWordCancle(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputWordCancleEvent>.Raise(new InputWordCancleEvent());
        }
    }
}
