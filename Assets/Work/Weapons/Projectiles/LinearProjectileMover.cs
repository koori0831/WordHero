namespace Work.Weapons.Projectiles
{
	public class LinearProjectileMover: BaseProjectileMover
	{
		public override void Start()
		{
			base.Start();

            _rb.linearVelocity = transform.forward * Speed;
        }
    }
}