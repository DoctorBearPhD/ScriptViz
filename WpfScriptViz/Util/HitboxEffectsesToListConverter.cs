using ScriptLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ScriptViz.Util
{
    public class HitboxEffectsesToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // HitboxEffects[] --> HitboxEffects --> Index
            var _in = value as HitboxEffects[];
            var _out = new List<string>();

            foreach (var item in _in)
            {
                if (item.IsEmpty())
                    _out.Add(item.Index.ToString());
                else
                    _out.Add(item.Index.ToString() + ": (empty)");
            }

            return _out;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back!");
        }
    }
}
