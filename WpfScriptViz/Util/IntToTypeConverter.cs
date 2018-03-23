using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ScriptLib;

namespace ScriptViz.Util
{
    public class IntToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // int to String
            return EnumUtil.GetEnumDescription((HitboxEffectTypeEnum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumUtil.GetEnumValueFromDescription(value as string);
        }
    }
}
