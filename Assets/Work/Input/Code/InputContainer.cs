using LitMotion;
using UnityEngine;
using UnityEngine.InputSystem;
using Work.Core.Utils.EventBus;

namespace Work.Input.Code
{
    public class InputContainer : Console.IPlayerActions, Console.IUIActions
    {
        private Console _console;

        public Vector2 MoveVector { get; private set; }
        public bool IsMovePressed { get; private set; }
        public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.KeyboardMouse;

        // 스틱 드리프트 방지
        private const float Deadzone = 0.2f;

        private MotionHandle _motionX;
        private MotionHandle _motionZ;

        private float _currentX;
        private float _currentZ;

        public void Init()
        {
            if (_console == null)
            {
                _console = new Console();
                _console.Player.SetCallbacks(this);
                _console.UI.SetCallbacks(this);
            }
            _console.Player.Enable();
            _console.UI.Enable();
            Bus<PlayerInputEnableEvent>.Events += SetPlayerInputEnable;
        }

        public void Deinit()
        {
            if (_motionX.IsActive()) _motionX.Cancel();
            if (_motionZ.IsActive()) _motionZ.Cancel();

            _console.Player.Disable();
            _console.UI.Disable();
            _console = null;
            Bus<PlayerInputEnableEvent>.Events -= SetPlayerInputEnable;
        }

        public void SetPlayerInputEnable(PlayerInputEnableEvent evt)
        {
            if (evt.Enable) _console.Player.Enable();
            else _console.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);

            Vector2 input = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
            Ease ease = context.canceled ? Ease.InQuad : Ease.OutQuad;
            if (input.sqrMagnitude < Deadzone * Deadzone) input = Vector2.zero;
            input = Vector2.ClampMagnitude(input, 1f);

            if (_motionX.IsActive()) _motionX.Cancel();
            _motionX = LMotion.Create(_currentX, input.x, 0.12f)
                .WithEase(ease)
                .Bind(x => {
                    _currentX = x;
                    UpdateMoveVector();
                });

            if (_motionZ.IsActive()) _motionZ.Cancel();
            _motionZ = LMotion.Create(_currentZ, input.y, 0.12f)
                .WithEase(ease)
                .Bind(z => {
                    _currentZ = z;
                    UpdateMoveVector();
                });
        }

        private void UpdateMoveVector()
        {
            // 계산된 X, Z 값을 합쳐서 최종 MoveVector 갱신
            MoveVector = new Vector2(_currentX, _currentZ);
            IsMovePressed = MoveVector.sqrMagnitude > 0.001f;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
            {
                Bus<InputInteractEvent>.Raise(new InputInteractEvent());
                Debug.Log("아니 시발 되잖아요");
            }
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

        public void OnWeaponSwap(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<WeaponSwapEvent>.Raise(new WeaponSwapEvent());
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<FirstWeaponSkillEvent>.Raise(new FirstWeaponSkillEvent());
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<SecondWeaponSkillEvent>.Raise(new SecondWeaponSkillEvent());
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

        #region UI Actions
        public void OnMenu(InputAction.CallbackContext context)
        {
            UpdateCurrentDevice(context);
            if (context.performed)
                Bus<InputMenuEvent>.Raise(new InputMenuEvent());
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}
