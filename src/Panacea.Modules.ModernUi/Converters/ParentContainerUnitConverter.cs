using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Panacea.Modules.ModernUi.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class ParentContainerUnitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var height = (double)values[0];
            if (height == 0.0) return 100.0;
            return height / 4 * (int)values[1] - 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


   
}
