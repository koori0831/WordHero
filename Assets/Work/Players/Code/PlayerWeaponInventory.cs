using Work.Weapons.Code;

namespace Work.Players.Code
{
    public sealed class PlayerWeaponInventory
    {
        public BaseWeapon CurrentWeapon { get; private set; }
        public BaseWeapon StandbyWeapon { get; private set; }

        public bool CanSwap => CurrentWeapon != null && StandbyWeapon != null && CurrentWeapon.IsSkillUsing != true;

        public BaseWeapon Equip(BaseWeapon newWeapon)
        {
            if (newWeapon == null) return null;

            if (CurrentWeapon == null)
            {
                CurrentWeapon = newWeapon;
                return null;
            }

            if (StandbyWeapon == null)
            {
                StandbyWeapon = CurrentWeapon;
                CurrentWeapon = newWeapon;
                return null;
            }

            BaseWeapon droppedWeapon = CurrentWeapon;
            CurrentWeapon = newWeapon;
            return droppedWeapon;
        }

        public bool Swap()
        {
            if (!CanSwap) return false;

            BaseWeapon temp = CurrentWeapon;
            CurrentWeapon = StandbyWeapon;
            StandbyWeapon = temp;
            return true;
        }

        public void SetOwner(Player owner)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.Owner = owner;
            }

            if (StandbyWeapon != null)
            {
                StandbyWeapon.Owner = owner;
            }
        }
    }
}
