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
            Debug.Log("mmingmingming");
            if (context.performed)
            {
                Bus<InputInteractEvent>.Raise(new InputInteractEvent());
                Debug.Log("아니 시발 되잖아요");
            }
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputMenuEvent>.Raise(new InputMenuEvent());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputAttackEvent>.Raise(new InputAttackEvent());
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (context.performed)
                Bus<InputDodgeEvent>.Raise(new InputDodgeEvent());
        }
    }
}
