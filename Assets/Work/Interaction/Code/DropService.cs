using UnityEngine;
using Work.Weapons.Code;

namespace Work.Interaction.Code
{
    public static class DropService
    {
        public static void DropWeapon(BaseWeapon weapon, Vector3 dropPosition)
        {
            if (weapon == null) return;

            weapon.transform.SetParent(null);
            weapon.transform.position = dropPosition;
            weapon.transform.rotation = Quaternion.identity;
            weapon.gameObject.SetActive(true);
            weapon.Owner = null;

            DroppedWeaponInteractable pickup = weapon.GetComponent<DroppedWeaponInteractable>();
            if (pickup == null)
            {
                pickup = weapon.gameObject.AddComponent<DroppedWeaponInteractable>();
            }

            pickup.Initialize(weapon);
        }
    }
}
