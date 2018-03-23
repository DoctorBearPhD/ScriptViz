using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScriptViz.Util
{
    [ValueConversion(typeof(Array), typeof(string))]
    public class HBFXTypesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0 ? $"{value} Types" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}