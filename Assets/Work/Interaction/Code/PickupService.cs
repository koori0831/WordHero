using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Interaction.Code
{
    public static class PickupService
    {
        public static void PickupWeapon(Player player, BaseWeapon weapon)
        {
            if (player == null || weapon == null) return;
            player.GetWeapon(weapon);
        }
    }
}
