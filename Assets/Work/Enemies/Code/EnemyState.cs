using Unity.Behavior;

namespace Work.Enemies.Code
{
    [BlackboardEnum]
    public enum EnemyState
    {
        NotFindTarget,
        FindTarget,
        Attack,
        Idle,
        Chase,
        Hit,
        Death,
        Wait,
    }
}
