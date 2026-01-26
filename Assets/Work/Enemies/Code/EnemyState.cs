using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
