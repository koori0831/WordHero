using UnityEngine;
using Work.Weapons.Code;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 투영 바빌론 시작 무기군과 기본 무기 프리팹 매핑 데이터.
    /// </summary>
    [CreateAssetMenu(fileName = "StarterWeaponCatalog", menuName = "SO/MainEntrance/StarterWeaponCatalog", order = 10)]
    public sealed class StarterWeaponCatalogSO : ScriptableObject
    {
        private const string DEFAULT_RESOURCE_PATH = "StarterWeaponCatalog";

        [SerializeField] private StarterWeaponOption[] weaponOptions;

        public StarterWeaponOption[] WeaponOptions => weaponOptions;

        /// <summary>
        /// 기본 Resources 경로에서 시작 무기 카탈로그 로드.
        /// </summary>
        /// <returns>기본 시작 무기 카탈로그.</returns>
        public static StarterWeaponCatalogSO LoadDefault()
        {
            return Resources.Load<StarterWeaponCatalogSO>(DEFAULT_RESOURCE_PATH);
        }

        /// <summary>
        /// 특정 무기군의 시작 무기 항목 조회.
        /// </summary>
        /// <param name="weaponType">조회할 무기군.</param>
        /// <param name="option">조회된 시작 무기 항목.</param>
        /// <returns>조회 성공 여부.</returns>
        public bool TryGetOption(WeaponType weaponType, out StarterWeaponOption option)
        {
            if (weaponOptions != null)
            {
                for (int i = 0; i < weaponOptions.Length; i++)
                {
                    if (weaponOptions[i].WeaponType == weaponType)
                    {
                        option = weaponOptions[i];
                        return true;
                    }
                }
            }

            option = StarterWeaponOption.CreateUnavailable(weaponType);
            return false;
        }

        /// <summary>
        /// 기본 무기 프리팹이 존재하는 무기군 개수 반환.
        /// </summary>
        /// <returns>선택 가능한 무기군 개수.</returns>
        public int CountAvailableOptions()
        {
            int count = 0;

            if (weaponOptions == null)
            {
                return count;
            }

            for (int i = 0; i < weaponOptions.Length; i++)
            {
                if (weaponOptions[i].IsAvailable)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
