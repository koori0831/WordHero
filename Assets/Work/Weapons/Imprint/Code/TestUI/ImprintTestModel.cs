using System.Collections.Generic;
using Work.Players.Code;
using Work.Weapons.Code;

namespace Work.Weapons.Imprint.Code.TestUI
{
    public class ImprintTestModel
    {
        private Player _player;

        public ImprintTestModel(Player player)
        {
            _player = player;
        }

        public BaseWeapon CurrentWeapon => _player?.WeaponModule?.CurrentWeapon;
        public BaseWeapon StandbyWeapon => _player?.WeaponModule?.StandbyWeapon;

        public List<ImprintWordSO> GetInventoryWords()
        {
            return _player?.ImprintWordInventory?.GetAllImprintWords() ?? new List<ImprintWordSO>();
        }

        public int GetWordAmount(ImprintWordSO word)
        {
            return _player?.ImprintWordInventory?.GetAmount(word) ?? 0;
        }

        public void EquipToWeapon(BaseWeapon weapon, ImprintWordSO word)
        {
            if (weapon == null || word == null) return;
            
            // 인벤토리에서 사용 가능한 경우에만 무기에 장착
            if (_player.ImprintWordInventory.TryUseImprintWord(word))
            {
                weapon.Imprints.TryEquip(word);
            }
        }
    }
}
