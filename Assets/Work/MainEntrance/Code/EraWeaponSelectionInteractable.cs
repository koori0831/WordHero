using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Core.Utils.EventBus;
using Work.Input.Code;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 에라 NPC를 통해 시작 무기군 2개를 선택하는 상호작용 컴포넌트.
    /// </summary>
    public sealed class EraWeaponSelectionInteractable : MonoBehaviour, IInteractable
    {
        private const int REQUIRED_SELECTION_COUNT = 2;
        private const float WINDOW_WIDTH = 460f;
        private const float WINDOW_HEIGHT = 390f;
        private const float WINDOW_MARGIN = 24f;
        private const float OPTION_BUTTON_HEIGHT = 34f;

        [SerializeField] private StarterWeaponCatalogSO weaponCatalog;
        [SerializeField] private string titleText = "에라의 병기고";
        [SerializeField] private string descriptionText = "출정 전에 무기군 2개를 선택";
        [SerializeField] private string confirmButtonText = "선택 완료";
        [SerializeField] private string cancelButtonText = "닫기";

        private List<WeaponType> _selectedWeaponTypes = new List<WeaponType>();
        private bool _isOpen;
        private bool _inputLocked;
        private string _statusMessage;

        /// <summary>
        /// 무기 선택 UI 열기.
        /// </summary>
        /// <param name="interactor">상호작용을 수행한 오브젝트.</param>
        public void Interact(GameObject interactor)
        {
            if (_isOpen)
            {
                return;
            }

            OpenSelection();
        }

        /// <summary>
        /// 컴포넌트 비활성화 시 입력 잠금 복구.
        /// </summary>
        private void OnDisable()
        {
            if (_isOpen)
            {
                CloseSelection();
            }
        }

        /// <summary>
        /// IMGUI 기반 임시 무기 선택 창 표시.
        /// </summary>
        private void OnGUI()
        {
            if (!_isOpen)
            {
                return;
            }

            float width = Mathf.Min(WINDOW_WIDTH, Screen.width - WINDOW_MARGIN * 2f);
            float height = Mathf.Min(WINDOW_HEIGHT, Screen.height - WINDOW_MARGIN * 2f);
            Rect windowRect = new Rect(
                (Screen.width - width) * 0.5f,
                (Screen.height - height) * 0.5f,
                width,
                height);

            GUI.ModalWindow(GetInstanceID(), windowRect, DrawSelectionWindow, titleText);
        }

        /// <summary>
        /// 선택 창 내부 UI 그리기.
        /// </summary>
        /// <param name="windowId">IMGUI 창 식별자.</param>
        private void DrawSelectionWindow(int windowId)
        {
            GUILayout.Label(descriptionText);
            GUILayout.Space(8f);
            GUILayout.Label($"선택됨: {_selectedWeaponTypes.Count}/{REQUIRED_SELECTION_COUNT}");
            GUILayout.Space(8f);

            WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));
            for (int i = 0; i < weaponTypes.Length; i++)
            {
                DrawWeaponOption(weaponTypes[i]);
            }

            GUILayout.FlexibleSpace();

            if (!string.IsNullOrWhiteSpace(_statusMessage))
            {
                GUILayout.Label(_statusMessage);
            }

            GUILayout.BeginHorizontal();
            GUI.enabled = _selectedWeaponTypes.Count == REQUIRED_SELECTION_COUNT;
            if (GUILayout.Button(confirmButtonText, GUILayout.Height(OPTION_BUTTON_HEIGHT)))
            {
                ConfirmSelection();
            }

            GUI.enabled = true;
            if (GUILayout.Button(cancelButtonText, GUILayout.Height(OPTION_BUTTON_HEIGHT)))
            {
                CloseSelection();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 무기군 선택 버튼 표시.
        /// </summary>
        /// <param name="weaponType">표시할 무기군.</param>
        private void DrawWeaponOption(WeaponType weaponType)
        {
            StarterWeaponOption option = GetOption(weaponType);
            bool isSelected = IsSelected(weaponType);
            bool canClick = option.IsAvailable && (isSelected || _selectedWeaponTypes.Count < REQUIRED_SELECTION_COUNT);
            string label = option.GetDisplayName();

            if (isSelected)
            {
                label = "[선택됨] " + label;
            }

            if (!option.IsAvailable)
            {
                label += " (준비 중)";
            }

            GUI.enabled = canClick;
            if (GUILayout.Button(label, GUILayout.Height(OPTION_BUTTON_HEIGHT)))
            {
                ToggleSelection(option);
            }
            GUI.enabled = true;
        }

        /// <summary>
        /// 무기 선택 창 열기 및 플레이어 입력 잠금.
        /// </summary>
        private void OpenSelection()
        {
            ResolveCatalog();
            _selectedWeaponTypes.Clear();
            _statusMessage = string.Empty;

            if (RunLoadoutState.IsComplete)
            {
                _selectedWeaponTypes.Add(RunLoadoutState.PrimaryOption.WeaponType);
                _selectedWeaponTypes.Add(RunLoadoutState.SecondaryOption.WeaponType);
            }

            _isOpen = true;
            _inputLocked = true;
            Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(false));
        }

        /// <summary>
        /// 무기 선택 창 닫기 및 플레이어 입력 복구.
        /// </summary>
        private void CloseSelection()
        {
            _isOpen = false;

            if (_inputLocked)
            {
                _inputLocked = false;
                Bus<PlayerInputEnableEvent>.Raise(new PlayerInputEnableEvent(true));
            }
        }

        /// <summary>
        /// 무기군 선택 상태 전환.
        /// </summary>
        /// <param name="option">선택 전환할 무기군 항목.</param>
        private void ToggleSelection(StarterWeaponOption option)
        {
            if (!option.IsAvailable)
            {
                return;
            }

            if (IsSelected(option.WeaponType))
            {
                _selectedWeaponTypes.Remove(option.WeaponType);
                _statusMessage = string.Empty;
                return;
            }

            if (_selectedWeaponTypes.Count >= REQUIRED_SELECTION_COUNT)
            {
                _statusMessage = "무기군은 2개까지만 선택 가능";
                return;
            }

            _selectedWeaponTypes.Add(option.WeaponType);
            _statusMessage = string.Empty;
        }

        /// <summary>
        /// 현재 선택된 2개 무기군을 런 시작 상태에 저장.
        /// </summary>
        private void ConfirmSelection()
        {
            if (_selectedWeaponTypes.Count != REQUIRED_SELECTION_COUNT)
            {
                _statusMessage = "무기군 2개를 선택 필요";
                return;
            }

            StarterWeaponOption primaryOption = GetOption(_selectedWeaponTypes[0]);
            StarterWeaponOption secondaryOption = GetOption(_selectedWeaponTypes[1]);

            if (!RunLoadoutState.SetLoadout(primaryOption, secondaryOption))
            {
                _statusMessage = "선택 저장 실패";
                return;
            }

            CloseSelection();
        }

        /// <summary>
        /// 기본 무기 카탈로그 참조 보정.
        /// </summary>
        private void ResolveCatalog()
        {
            if (weaponCatalog != null)
            {
                return;
            }

            weaponCatalog = StarterWeaponCatalogSO.LoadDefault();
            if (weaponCatalog == null)
            {
                Debug.LogWarning("Starter weapon catalog is missing. Create Resources/StarterWeaponCatalog asset.");
            }
        }

        /// <summary>
        /// 무기군 항목 조회.
        /// </summary>
        /// <param name="weaponType">조회할 무기군.</param>
        /// <returns>조회된 무기군 항목.</returns>
        private StarterWeaponOption GetOption(WeaponType weaponType)
        {
            ResolveCatalog();

            if (weaponCatalog != null && weaponCatalog.TryGetOption(weaponType, out StarterWeaponOption option))
            {
                return option;
            }

            return StarterWeaponOption.CreateUnavailable(weaponType);
        }

        /// <summary>
        /// 무기군 선택 여부 반환.
        /// </summary>
        /// <param name="weaponType">확인할 무기군.</param>
        /// <returns>선택 여부.</returns>
        private bool IsSelected(WeaponType weaponType)
        {
            return _selectedWeaponTypes.Contains(weaponType);
        }
    }
}
