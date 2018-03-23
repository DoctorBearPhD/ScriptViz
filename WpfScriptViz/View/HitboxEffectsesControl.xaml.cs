using System.Collections.Generic;
using System.Windows.Controls;

namespace ScriptViz.View
{
    /// <summary>
    /// Interaction logic for HitboxEffectsesControl.xaml
    /// </summary>
    public partial class HitboxEffectsesControl
    {
        private readonly string[] _hbfxTypeNames =
        {
            "HIT__STAND",
            "HIT__CROUCH",
            "HIT__AIR",
            "HIT__UNKNOWN",
            "HIT__UNKNOWN2",
            "GUARD__STAND",
            "GUARD__CROUCH",
            "GUARD__AIR",
            "GUARD__UNKNOWN",
            "GUARD__UNKNOWN2",
            "COUNTERHIT__STAND",
            "COUNTERHIT__CROUCH",
            "COUNTERHIT__AIR",
            "COUNTERHIT__UNKNOWN",
            "COUNTERHIT__UNKNOWN2",
            "UNKNOWN__STAND",
            "UNKNOWN__CROUCH",
            "UNKNOWN__AIR",
            "UNKNOWN__UNKNOWN",
            "UNKNOWN__UNKNOWN2"
        };

        public HitboxEffectsesControl()
        {
            InitializeComponent();
        }

        private void HitboxEffectsContent_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = _hbfxTypeNames[e.Row.GetIndex()];
        }
    }
}
