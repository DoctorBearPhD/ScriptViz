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
    public class MovesToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Move[] moves = (value as MoveList).Moves;
            var names = new List<string>();
            int i = 0;

            foreach (var move in moves)
            {
                if (move != null) names.Add(move.Name);
                else names.Add("null (Script Index: " + i + ")");
                i++;
            }

            return names;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back!");
        }
    }
}
