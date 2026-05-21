using UnityEngine;
using Work.Core.Utils.EventBus;

namespace Work.MainEntrance.Code
{
    /// <summary>
    /// 씬 전환 동안 유지되는 런 시작 무기 선택 상태.
    /// </summary>
    public static class RunLoadoutState
    {
        private static StarterWeaponOption _primaryOption;
        private static StarterWeaponOption _secondaryOption;

        public static StarterWeaponOption PrimaryOption => _primaryOption;
        public static StarterWeaponOption SecondaryOption => _secondaryOption;
        public static bool IsComplete => _primaryOption.IsAvailable && _secondaryOption.IsAvailable;

        /// <summary>
        /// 시작 무기 2개 선택 저장.
        /// </summary>
        /// <param name="primaryOption">주 무기 항목.</param>
        /// <param name="secondaryOption">보조 무기 항목.</param>
        /// <returns>저장 성공 여부.</returns>
        public static bool SetLoadout(StarterWeaponOption primaryOption, StarterWeaponOption secondaryOption)
        {
            if (!primaryOption.IsAvailable || !secondaryOption.IsAvailable)
            {
                Debug.LogWarning("Run loadout selection failed: starter weapon prefab is missing.");
                return false;
            }

            if (primaryOption.WeaponType == secondaryOption.WeaponType)
            {
                Debug.LogWarning("Run loadout selection failed: duplicate weapon type selected.");
                return false;
            }

            _primaryOption = primaryOption;
            _secondaryOption = secondaryOption;
            Bus<RunLoadoutChangedEvent>.Raise(new RunLoadoutChangedEvent(_primaryOption, _secondaryOption, IsComplete));
            return true;
        }

        /// <summary>
        /// 시작 무기 선택 상태 초기화.
        /// </summary>
        public static void Clear()
        {
            _primaryOption = default;
            _secondaryOption = default;
            Bus<RunLoadoutChangedEvent>.Raise(new RunLoadoutChangedEvent(_primaryOption, _secondaryOption, false));
        }
    }

    /// <summary>
    /// 런 시작 무기 선택 상태가 변경될 때 발생하는 이벤트.
    /// </summary>
    /// <param name="PrimaryOption">주 무기 항목.</param>
    /// <param name="SecondaryOption">보조 무기 항목.</param>
    /// <param name="IsComplete">무기 2개 선택 완료 여부.</param>
    public readonly record struct RunLoadoutChangedEvent(
        StarterWeaponOption PrimaryOption,
        StarterWeaponOption SecondaryOption,
        bool IsComplete) : IEvent;

    /// <summary>
    /// 런 시작 조건을 만족하지 못해 출정이 차단될 때 발생하는 이벤트.
    /// </summary>
    /// <param name="Message">차단 안내 문구.</param>
    public readonly record struct RunLoadoutRequirementFailedEvent(string Message) : IEvent;
}
