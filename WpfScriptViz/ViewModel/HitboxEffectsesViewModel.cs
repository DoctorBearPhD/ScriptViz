using ScriptLib;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using ScriptViz.Util;

namespace ScriptViz.ViewModel
{
    public class HitboxEffectsesViewModel : TabItemViewModel
    {
        #region Variables

        public string[] TypeList { get; } = {
            EnumUtil.GetEnumDescription((HitboxEffectTypeEnum) 0),
            EnumUtil.GetEnumDescription((HitboxEffectTypeEnum) 1),
            EnumUtil.GetEnumDescription((HitboxEffectTypeEnum) 2),
            EnumUtil.GetEnumDescription((HitboxEffectTypeEnum) 3),
            EnumUtil.GetEnumDescription((HitboxEffectTypeEnum) 4)
        };

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

        public HitboxEffects SelectedHitboxEffects => ( this.Content as HitboxEffects[] )?[SelectedHitboxEffectsIndex];

        private bool _isUnknownsVisible = true;

        public bool IsUnknownsVisible
        {
            get => _isUnknownsVisible;
            set { _isUnknownsVisible = value; RaisePropertyChanged(nameof(IsUnknownsVisible)); }
        }


        #endregion // Variables

        #region Methods

        public HitboxEffectsesViewModel()
        {
            Messenger.Default.Register<bool>(this, nameof(IsUnknownsVisible), f => IsUnknownsVisible = f);
        }

        #endregion
    }
}
