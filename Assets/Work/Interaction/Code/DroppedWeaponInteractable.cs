using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Interaction.Code
{
    public class DroppedWeaponInteractable : MonoBehaviour, IInteractable
    {
        private const string PickupTriggerObjectName = "DropPickupTrigger";

        [SerializeField] private float pickupColliderRadius = 0.8f;

        private BaseWeapon _weapon;
        private Transform _pickupTriggerRoot;
        private SphereCollider _pickupCollider;

        public void Initialize(BaseWeapon weapon)
        {
            _weapon = weapon;

            if (_pickupTriggerRoot == null)
            {
                Transform existingTrigger = transform.Find(PickupTriggerObjectName);
                if (existingTrigger != null)
                {
                    _pickupTriggerRoot = existingTrigger;
                }
                else
                {
                    GameObject triggerObject = new GameObject(PickupTriggerObjectName);
                    _pickupTriggerRoot = triggerObject.transform;
                    _pickupTriggerRoot.SetParent(transform, false);
                }
            }

            _pickupTriggerRoot.localPosition = Vector3.zero;
            _pickupTriggerRoot.localRotation = Quaternion.identity;
            _pickupTriggerRoot.localScale = GetInverseLossyScale(transform.lossyScale);

            if (_pickupCollider == null)
            {
                _pickupCollider = _pickupTriggerRoot.GetComponent<SphereCollider>();
                if (_pickupCollider == null)
                {
                    _pickupCollider = _pickupTriggerRoot.gameObject.AddComponent<SphereCollider>();
                }
            }

            _pickupCollider.isTrigger = true;
            _pickupCollider.radius = pickupColliderRadius;
            _pickupCollider.enabled = true;
        }

        public void Interact(GameObject interactor)
        {

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

            if (_pickupTriggerRoot != null)
            {
                Destroy(_pickupTriggerRoot.gameObject);
                _pickupTriggerRoot = null;
            }
        }

        private static Vector3 GetInverseLossyScale(Vector3 lossyScale)
        {
            const float epsilon = 0.0001f;

            float x = Mathf.Abs(lossyScale.x) > epsilon ? 1f / lossyScale.x : 1f;
            float y = Mathf.Abs(lossyScale.y) > epsilon ? 1f / lossyScale.y : 1f;
            float z = Mathf.Abs(lossyScale.z) > epsilon ? 1f / lossyScale.z : 1f;

            return new Vector3(x, y, z);
        }
    }
}
