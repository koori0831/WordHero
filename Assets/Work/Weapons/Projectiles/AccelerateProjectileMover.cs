using UnityEngine;

namespace Work.Weapons.Projectiles
{
	public class AccelerateProjectileMover: BaseProjectileMover
	{
        private void FixedUpdate()
        {
            _rb.AddForce(transform.forward * Speed * Time.deltaTime * 100, ForceMode.Acceleration);
        }
    }
}