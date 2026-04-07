namespace Work.Weapons.Imprint.Code.TestUI
{
    public class ImprintTestPresenter
    {
        private ImprintTestModel _model;
        private ImprintTestView _view;

        public ImprintTestPresenter(ImprintTestModel model, ImprintTestView view)
        {
            _model = model;
            _view = view;

            _view.OnEquipToCurrentWeapon += HandleEquipToCurrent;
            _view.OnEquipToStandbyWeapon += HandleEquipToStandby;
        }

        private void HandleEquipToCurrent(ImprintWordSO word)
        {
            _model.EquipToWeapon(_model.CurrentWeapon, word);
        }

        private void HandleEquipToStandby(ImprintWordSO word)
        {
            _model.EquipToWeapon(_model.StandbyWeapon, word);
        }
    }
}
