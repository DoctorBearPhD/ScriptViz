using ScriptLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ScriptViz.Util
{
    class MoveToGeneralPropertiesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new List<object>();

            var move = value as Move;

            var props = new List<KeyValuePair<string, object>>();

            foreach (var prop in move.GetGeneralProperties())
            {
                props.Add(new KeyValuePair<string, object>(prop.Name, prop.GetValue(move)));
            }

            return props;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back!");
        }
    }
}
