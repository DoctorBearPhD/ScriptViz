using ScriptLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ScriptViz.Util
{
    public class HitboxEffectsToContentConverter : IValueConverter
    {
        // Simplified as much as possible, though not reader-friendly!
        // If the value is not a HitboxEffects, or if the HitboxEffects is full of nulls, return null. Otherwise return its HitboxEffectTypes.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            !(value is HitboxEffects item) || item.IsEmpty() ? null : item.GetHitboxEffectTypes();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
