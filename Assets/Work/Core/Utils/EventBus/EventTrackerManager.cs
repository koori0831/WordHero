using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Work.Core.Utils.EventBus
{
    public static class EventTrackerManager
    {
        public static Action<string> OnGlobalLog; // 모든 이벤트를 수집하는 전역 액션

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            // 1. 현재 어셈블리에서 IEvent를 상속받은 모든 타입을 찾습니다.
            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IEvent).IsAssignableFrom(p) && !p.IsInterface);

            foreach (var type in eventTypes)
            {
                // 2. 해당 타입을 사용하는 Bus<T> 클래스 타입을 만듭니다. (예: Bus<PlayerJumpedEvent>)
                var busType = typeof(Bus<>).MakeGenericType(type);

                // 3. Bus<T>.OnLog 필드를 가져옵니다.
                var onLogField = busType.GetField("OnLog", BindingFlags.Public | BindingFlags.Static);

                if (onLogField != null)
                {
                    // 4. OnLog에 연결될 대리자(Delegate)를 생성합니다.
                    Action<string> logDelegate = (log) => OnGlobalLog?.Invoke(log);

                    // 5. 이미 등록된 값을 가져와서 추가(Combine)합니다.
                    var currentValue = (Action<string>)onLogField.GetValue(null);
                    onLogField.SetValue(null, Delegate.Combine(currentValue, logDelegate));
                }
            }

            Debug.Log($"<color=green><b>[EventBus]</b></color> {eventTypes.Count()}개의 이벤트를 자동으로 추적 중입니다.");
        }
    }
}