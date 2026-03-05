using Work.Combat.Code;

namespace Work.Entities
{
    public interface IDamageable : ICastable
    {
        public void TakeDamage(int damageAmount);
    }
}
