namespace Work.Combat.Code
{
    public interface IDamageable : ICastable
    {
        void TakeDamage(int damageAmount);
    }
}
