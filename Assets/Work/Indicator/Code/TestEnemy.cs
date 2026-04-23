using UnityEngine;
using Work.Enemies.Code;

namespace Work.Indicator.Code
{
    /// <summary>
    /// 인디케이터 테스트를 위한 가짜 적 클래스
    /// </summary>
    public class TestEnemy : Enemy
    {
        public override void Init()
        {
            // 부모의 무거운 Init을 호출하지 않고 테스트에 필요한 값만 설정
            IsCanShowInfo = false;
            IsDead = false;
        }

        public override void VariableSetting() { } // 에러 방지를 위해 비워둠

        public void Kill()
        {
            IsDead = true;
        }
    }
}
