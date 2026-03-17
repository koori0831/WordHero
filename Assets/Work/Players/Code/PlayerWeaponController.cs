using UnityEngine;
using Work.Interaction.Code;
using Work.Weapons.Code;

namespace Work.Players.Code
{
    public sealed class PlayerWeaponController
    {
        private readonly Transform _weaponHandler;

        public PlayerWeaponController(Transform weaponHandler)
        {
            _weaponHandler = weaponHandler;
        }

        public void ApplyVisualState(BaseWeapon currentWeapon, BaseWeapon standbyWeapon)
        {
            if (currentWeapon == null || _weaponHandler == null) return;

            if (standbyWeapon != null)
            {
                standbyWeapon.gameObject.SetActive(false);
            }

            currentWeapon.gameObject.SetActive(true);
            RemoveDropMarker(currentWeapon);
            AttachToHandler(currentWeapon);
        }

        private void AttachToHandler(BaseWeapon weapon)
        {
            weapon.transform.SetParent(_weaponHandler);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;

            if (weapon.Data != null)
            {
                _weaponHandler.localPosition = weapon.Data.HandlerPosition;
                _weaponHandler.localRotation = Quaternion.Euler(weapon.Data.HandlerRotation);
            }
        }

        private static void RemoveDropMarker(BaseWeapon weapon)
        {
            DroppedWeaponInteractable droppedWeapon = weapon.GetComponent<DroppedWeaponInteractable>();
            if (droppedWeapon == null) return;

            droppedWeapon.CleanupDropComponents();
            Object.Destroy(droppedWeapon);
        }
    }
}
