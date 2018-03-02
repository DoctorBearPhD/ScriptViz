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
            Move[] moves = (value as Move[]);
            var listItems = new List<string>();
            
            for(int i = 0; i < moves.Length; i++)
            {
                Move move = moves[i];
                if (move != null)
                    listItems.Add( move.Index + ": " + move.Name );
                else listItems.Add( i + ": null" );
                
            }

            return listItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Can't convert back!");
        }
    }
}
