using Work.Core.Utils.EventBus;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 적 스폰 시 발생하는 이벤트
    /// </summary>
    public readonly record struct OnEnemySpawnEvent(Enemy Enemy) : IEvent;

    /// <summary>
    /// 적 소멸 시 발생하는 이벤트
    /// </summary>
    public readonly record struct OnEnemyDestroyEvent(Enemy Enemy) : IEvent;
}
