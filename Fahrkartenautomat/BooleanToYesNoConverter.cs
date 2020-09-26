using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Fahrkartenautomat
{
    [ValueConversion(typeof(bool), typeof(string))]
    class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boo = value != null && (bool)value;
            return boo ? "Ja" : "Nein"; // Nicht gut ich weiß
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return str == "Ja"; // Auch nicht gut
        }
 
    }
}
