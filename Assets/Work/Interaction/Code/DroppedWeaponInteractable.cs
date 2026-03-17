using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Interaction.Code
{
    public class DroppedWeaponInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float pickupColliderRadius = 0.8f;

        private BaseWeapon _weapon;
        private SphereCollider _pickupCollider;

        public void Initialize(BaseWeapon weapon)
        {
            _weapon = weapon;

            if (_pickupCollider == null)
            {
                _pickupCollider = gameObject.AddComponent<SphereCollider>();
            }

            _pickupCollider.isTrigger = true;
            _pickupCollider.radius = pickupColliderRadius;
            _pickupCollider.enabled = true;
        }

        public void Interact(GameObject interactor)
        {
            if (_weapon == null) return;

            if (interactor.TryGetComponent(out Player player))
            {
                PickupService.PickupWeapon(player, _weapon);
            }
        }

        public void CleanupDropComponents()
        {
            if (_pickupCollider != null)
            {
                Destroy(_pickupCollider);
                _pickupCollider = null;
            }
        }
    }
}
