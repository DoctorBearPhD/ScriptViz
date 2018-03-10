using ScriptLib;
using System.Collections.Generic;
using System.Linq;

namespace ScriptViz.ViewModel
{
    public class HitboxEffectsesViewModel : TabItemViewModel
    {
        #region Variables

        int mSelectedHitboxEffectsIndex;
        public int SelectedHitboxEffectsIndex
        {
            get => mSelectedHitboxEffectsIndex;
            set
            {
                mSelectedHitboxEffectsIndex = value;
                RaisePropertyChanged(nameof(SelectedHitboxEffectsIndex));
                RaisePropertyChanged(nameof(SelectedHitboxEffects));
            }
        }

        public HitboxEffects SelectedHitboxEffects
        {
            get => ( this.Content as HitboxEffects[] )[SelectedHitboxEffectsIndex];
        }

        #endregion // Variables

        #region Methods

        #endregion
    }
}
