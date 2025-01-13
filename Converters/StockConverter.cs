using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Datenlotsen.Converters
{
    public class StockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool stockStatus)
            {
                return stockStatus ? "In Stock" : "Out of Stock";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stockStatus)
            {
                if (stockStatus == "In Stock")
                {
                    return true;
                }
                else if (stockStatus == "Out of Stock")
                {
                    return false;
                }
            }
            return false;
        }
    }
}
