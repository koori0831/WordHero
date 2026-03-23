using UnityEngine;

namespace Work.Weapons.Projectiles
{
	public class AccelerateProjectileMover: BaseProjectileMover
	{
        private void Update()
        {
            _rb.AddForce(transform.forward * Speed, ForceMode.Acceleration);
        }
    }
}