using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ItemEditor.Code
{
    public class SpritesheetScaleConverter : IValueConverter
    {
        public static double SpritesheetWidth = 476;

        //---------------------------------------------------------------------------

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            return (SpritesheetWidth != 0.0 ? width / SpritesheetWidth : 1.0);
        }

        //---------------------------------------------------------------------------

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
