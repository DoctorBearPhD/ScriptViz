using ScriptLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ScriptViz.Util
{
    public class HitboxEffectsToContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // first parameter is the list of HitboxEffects, second is the list's selected index
            var item = (value as HitboxEffects);

            if (item.IsEmpty())
                return item.GetHitboxEffectTypes();
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
