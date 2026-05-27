using System;
using System.Collections.Generic;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 에라의 병기고에 시작 무기 전시대를 배치하고 선택 상태를 관리하는 컴포넌트
    /// </summary>
    public sealed class EraWeaponSelectionInteractable : MonoBehaviour
    {
        private const int REQUIRED_SELECTION_COUNT = 2;

        [SerializeField] private StarterWeaponCatalogSO weaponCatalog;
        [SerializeField] private Transform displayRoot;
        [SerializeField] private Vector3 firstStandLocalPosition = new Vector3(-4.8f, 0f, 0f);
        [SerializeField] private float standSpacing = 2.4f;
        [SerializeField] private Vector3 displayLocalPosition = new Vector3(0f, 1.15f, 0f);
        [SerializeField] private Vector3 displayLocalEulerAngles = new Vector3(0f, 45f, 35f);
        [SerializeField] private Vector3 displayLocalScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private Vector3 labelLocalPosition = new Vector3(0f, 2.15f, 0f);
        [SerializeField] private Vector3 interactionColliderSize = new Vector3(1.7f, 2.4f, 1.7f);
        [SerializeField] private string maxSelectionMessage = "시작 무기군은 2개까지만 선택 가능";

        private List<StarterWeaponDisplayStand> _stands = new List<StarterWeaponDisplayStand>();
        private List<StarterWeaponOption> _selectedOptions = new List<StarterWeaponOption>();

        /// <summary>
        /// 시작 무기 전시대 생성
        /// </summary>
        private void Start()
        {
            BuildDisplayStands();
        }

        /// <summary>
        /// 특정 전시대의 선택 상태 전환
        /// </summary>
        /// <param name="stand">선택 상태를 전환할 전시대</param>
        /// <returns>선택 상태 전환 성공 여부</returns>
        public bool TryToggleSelection(StarterWeaponDisplayStand stand)
        {
            if (stand == null || !stand.Option.IsAvailable)
            {
                return false;
            }

            StarterWeaponOption option = stand.Option;
            if (IsSelected(option.WeaponType))
            {
                RemoveSelection(option.WeaponType);
                ApplySelectionState();
                return true;
            }

            if (_selectedOptions.Count >= REQUIRED_SELECTION_COUNT)
            {
                Debug.Log(maxSelectionMessage);
                return false;
            }

            _selectedOptions.Add(option);
            ApplySelectionState();
            return true;
        }

        /// <summary>
        /// 무기군 선택 여부 반환
        /// </summary>
        /// <param name="weaponType">확인할 무기군</param>
        /// <returns>선택 여부</returns>
        public bool IsSelected(WeaponType weaponType)
        {
            return FindSelectedIndex(weaponType) >= 0;
        }

        /// <summary>
        /// 카탈로그 기준 전시대 배치
        /// </summary>
        private void BuildDisplayStands()
        {
            if (_stands.Count > 0)
            {
                return;
            }

            ResolveCatalog();
            Transform root = displayRoot != null ? displayRoot : transform;
            WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));

            for (int i = 0; i < weaponTypes.Length; i++)
            {
                StarterWeaponOption option = GetOption(weaponTypes[i]);
                GameObject standObject = new GameObject($"{option.GetDisplayName()} DisplayStand");
                standObject.transform.SetParent(root);
                standObject.transform.localPosition = firstStandLocalPosition + new Vector3(standSpacing * i, 0f, 0f);
                standObject.transform.localRotation = Quaternion.identity;
                standObject.transform.localScale = Vector3.one;

                StarterWeaponDisplayStand stand = standObject.AddComponent<StarterWeaponDisplayStand>();
                stand.Initialize(
                    this,
                    option,
                    displayLocalPosition,
                    displayLocalEulerAngles,
                    displayLocalScale,
                    labelLocalPosition,
                    interactionColliderSize);

                _stands.Add(stand);
            }

            if (RunLoadoutState.IsComplete)
            {
                _selectedOptions.Add(RunLoadoutState.PrimaryOption);
                _selectedOptions.Add(RunLoadoutState.SecondaryOption);
            }

            RefreshStandVisuals();
        }

        /// <summary>
        /// 현재 선택 상태를 런 시작 상태에 반영
        /// </summary>
        private void ApplySelectionState()
        {
            if (_selectedOptions.Count == REQUIRED_SELECTION_COUNT)
            {
                RunLoadoutState.SetLoadout(_selectedOptions[0], _selectedOptions[1]);
            }
            else
            {
                RunLoadoutState.Clear();
            }

            RefreshStandVisuals();
        }

        /// <summary>
        /// 모든 전시대의 선택 표시 갱신
        /// </summary>
        private void RefreshStandVisuals()
        {
            for (int i = 0; i < _stands.Count; i++)
            {
                _stands[i].SetSelected(IsSelected(_stands[i].Option.WeaponType));
            }
        }

        /// <summary>
        /// 선택 목록에서 무기군 제거
        /// </summary>
        /// <param name="weaponType">제거할 무기군</param>
        private void RemoveSelection(WeaponType weaponType)
        {
            int selectedIndex = FindSelectedIndex(weaponType);
            if (selectedIndex < 0)
            {
                return;
            }

            _selectedOptions.RemoveAt(selectedIndex);
        }

        /// <summary>
        /// 선택 목록에서 무기군 인덱스 조회
        /// </summary>
        /// <param name="weaponType">조회할 무기군</param>
        /// <returns>선택 목록 인덱스</returns>
        private int FindSelectedIndex(WeaponType weaponType)
        {
            for (int i = 0; i < _selectedOptions.Count; i++)
            {
                if (_selectedOptions[i].WeaponType == weaponType)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 기본 무기 카탈로그 참조 보정
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
                Debug.LogWarning("Starter weapon catalog is missing. Assign StarterWeaponCatalogSO to EraWeaponSelectionInteractable.");
            }
        }

        /// <summary>
        /// 무기군 항목 조회
        /// </summary>
        /// <param name="weaponType">조회할 무기군</param>
        /// <returns>조회된 무기군 항목</returns>
        private StarterWeaponOption GetOption(WeaponType weaponType)
        {
            ResolveCatalog();

            if (weaponCatalog != null && weaponCatalog.TryGetOption(weaponType, out StarterWeaponOption option))
            {
                return option;
            }

            return StarterWeaponOption.CreateUnavailable(weaponType);
        }
    }
}
