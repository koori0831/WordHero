using System;
using UnityEngine;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Interaction.Code
{
    [Serializable]
    public class WeaponCollectAction : ICollectAction
    {
        public BaseWeapon WeaponPrefab;
        private BaseWeapon WeaponInstance;

        public void Collect(Player collector)
        {
            WeaponInstance.gameObject.SetActive(true);
            PickupService.PickupWeapon(collector, WeaponInstance);
        }

        public void Initialize()
        {
            WeaponInstance = GameObject.Instantiate(WeaponPrefab);
            WeaponInstance.gameObject.SetActive(false);
        }
    }
}
