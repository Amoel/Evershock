using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TilesetViewer
{
    public class BoolToTintConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new BrushConverter().ConvertFromString("#44FF0000") : new BrushConverter().ConvertFromString("#4400FF00");
        }

        //---------------------------------------------------------------------------

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
