using ScriptLib;

namespace ScriptViz.ViewModel
{
    public class HitboxEffectsesViewModel : TabItemViewModel
    {
        #region Variables

        private int _selectedHitboxEffectsIndex;
        public int SelectedHitboxEffectsIndex
        {
            get => _selectedHitboxEffectsIndex;
            set
            {
                _selectedHitboxEffectsIndex = value;
                RaisePropertyChanged(nameof(SelectedHitboxEffectsIndex));
                RaisePropertyChanged(nameof(SelectedHitboxEffects));
            }
        }

        public HitboxEffects SelectedHitboxEffects => (this.Content as HitboxEffects[])?[SelectedHitboxEffectsIndex];

        #endregion // Variables

        #region Methods

        #endregion
    }
}
