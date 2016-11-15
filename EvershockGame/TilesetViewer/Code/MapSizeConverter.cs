using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TilesetViewer
{
    public class MapSizeConverter : IValueConverter
    {
        private static float m_Zoom = 1.0f;

        //---------------------------------------------------------------------------

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * (1.0f / m_Zoom);
        }

        //---------------------------------------------------------------------------

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        //---------------------------------------------------------------------------

        public static void UpdateZoom(float zoom)
        {
            m_Zoom = zoom;
        }
    }
}
