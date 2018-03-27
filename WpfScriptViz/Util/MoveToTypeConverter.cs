using ScriptLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace ScriptViz.Util
{
    public class MoveToTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null || values[1] == null) return null;

            var move = values[0] as Move;
            var prop = values[1] as PropertyInfo;
            // var typePropIndex = (int) values[2];

            var props = new List<object>();

            if (prop == null) return props;
            if (prop.GetValue(move) == null) return props;

            var objects = (object[])prop.GetValue(move);

            foreach (var t in objects)
            {
                props.Add(t);
            }

            return props;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back!");
        }
    }
}
