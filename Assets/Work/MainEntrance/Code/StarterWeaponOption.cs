using System;
using UnityEngine;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 투영 바빌론에서 선택 가능한 시작 무기군 항목.
    /// </summary>
    [Serializable]
    public struct StarterWeaponOption
    {
        public WeaponType WeaponType;
        public string DisplayName;
        public BaseWeapon WeaponPrefab;

        public bool IsAvailable => WeaponPrefab != null;

        /// <summary>
        /// UI에 표시할 무기군 이름 반환.
        /// </summary>
        /// <returns>표시용 무기군 이름.</returns>
        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
            {
                return DisplayName;
            }

            switch (WeaponType)
            {
                case WeaponType.OneHandSword:
                    return "한손검";
                case WeaponType.TwoHandSword:
                    return "양손검";
                case WeaponType.Axe:
                    return "도끼";
                case WeaponType.Polearm:
                    return "장병기";
                case WeaponType.Blunt:
                    return "둔기";
                default:
                    return WeaponType.ToString();
            }
        }

        /// <summary>
        /// 프리팹이 없는 비활성 무기군 항목 생성.
        /// </summary>
        /// <param name="weaponType">비활성으로 표시할 무기군.</param>
        /// <returns>비활성 무기군 항목.</returns>
        public static StarterWeaponOption CreateUnavailable(WeaponType weaponType)
        {
            return new StarterWeaponOption
            {
                WeaponType = weaponType,
                DisplayName = string.Empty,
                WeaponPrefab = null
            };
        }
    }
}
