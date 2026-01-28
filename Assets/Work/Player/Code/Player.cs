using Code.Entities;
using Code.FSM;
using Work.Entities;

namespace Work.Player.Code
{
    public class Player : Entity, IDamageable
    {
        private EntityHealth _health;
        private StateMachine _stateMachine;

        private void OnEnable()
        {
            _health = GetCompo<EntityHealth>();
            _stateMachine = GetCompo<StateCompo>().StateMachine;

            _health.DeadTrigger += OnDead;
            _health.DamagedTrigger += OnHit;
        }

        private void OnDisable()
        {
            _health.DeadTrigger -= OnDead;
            _health.DamagedTrigger -= OnHit;
        }

        private void OnDead() => _stateMachine.ChangeState("Death");
        private void OnHit() => _stateMachine.ChangeState("Hit");

        public void TakeDamage(int damageAmount)
        {
            _health.DecreaseHP(damageAmount);
        }
    }
}